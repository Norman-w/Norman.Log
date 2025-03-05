#!/bin/bash

# 使用Docker部署该应用的脚本，使用Teamcity调用该脚本进行部署
# 如果中途出现错误，则停止部署

# 定义变量
IMAGE_NAME="norman.log.server:1.0"
CONTAINER_NAME="norman.log.server"
PORT=10007

# 日志输出函数
log_info() {
  echo "[INFO] $1"
}

log_error() {
  echo "[ERROR] $1"
}

# 检查并释放端口
release_port() {
  local port=$1
  while lsof -i:$port -sTCP:LISTEN &>/dev/null; do
    log_info "检测到端口 $port 正被占用，尝试释放..."
    PID=$(lsof -ti:$port -sTCP:LISTEN)
    if [ -n "$PID" ]; then
      log_info "杀死占用端口 $port 的进程 PID: $PID"
      kill -9 $PID || { log_error "无法释放端口 $port，部署失败"; exit 1; }
      sleep 2 # 等待端口释放
    fi
  done
  log_info "端口 $port 已释放，可以继续进行流程"
}

# 构建镜像
log_info "开始根据 Dockerfile 构建镜像..."
docker build .. -f ../Norman.Log.Server/Dockerfile -t $IMAGE_NAME || { log_error "镜像构建失败"; exit 1; }
log_info "镜像已成功构建: $IMAGE_NAME"

# 停止并删除现有容器
log_info "停止并删除容器: $CONTAINER_NAME"
docker stop $CONTAINER_NAME &>/dev/null
docker rm $CONTAINER_NAME &>/dev/null
log_info "容器已停止并删除"

# 确认端口未被占用
release_port $PORT

# 启动新的容器
log_info "启动新的容器..."
docker run -d -p $PORT:$PORT --name $CONTAINER_NAME $IMAGE_NAME || { log_error "容器启动失败"; exit 1; }
log_info "容器启动命令已执行"

# 检测端口是否在监听
log_info "检测端口 $PORT 是否正在监听..."
COUNT=0
MAX_RETRIES=10
SLEEP_INTERVAL=2

while ! lsof -i:$PORT -sTCP:LISTEN &>/dev/null; do
  sleep $SLEEP_INTERVAL
  COUNT=$((COUNT + 1))
  log_info "端口 $PORT 尚未监听，重试次数: $COUNT/$MAX_RETRIES"
  if [ "$COUNT" -ge "$MAX_RETRIES" ]; then
    log_error "容器未能在 $PORT 上启动监听"
    exit 1
  fi
done
log_info "端口 $PORT 正在监听"

# 验证容器状态
log_info "验证容器运行状态..."
if docker ps | grep -q $CONTAINER_NAME; then
  log_info "容器 $CONTAINER_NAME 已成功启动并正常运行"
  exit 0
else
  log_error "容器 $CONTAINER_NAME 未能正常运行"
  exit 1
fi