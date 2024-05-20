const ws = require("nodejs-websocket");

console.log("WS服务开始建立连接...")

const connects = {
  server: null,
  client: null,
  record: null,
};

const connectReady = {
  server: null,
  client: null,
  record: null,
};

const roleType = {
  server: 'server', // 发送和录制数据
  client: 'client', // 只接收回放
  record: 'record', // 只发送录制数据
}

const ignoreType = ["connect", "deploy"]

const send = (fromRole, toRole, text) => {
  var conn = connects[toRole];
  if (conn) {
    conn.sendText(text);
  }
}

ws.createServer((conn) => {
  conn.on('listening', (code, reason) => { 
    console.log("建立连接", code, reason)
  })

  conn.on("text", (source) => {
    var role = roleType.server;
    var sourceData = {};
    var data = "";
    try {
      sourceData = JSON.parse(source)
      console.error("sourceData: ", sourceData)
      console.log("sourceData: ", typeof sourceData.data)
      role = sourceData.role;
      if (sourceData.op == 'subscribe' && sourceData.topic == '/metapose/pose3d') { 
        role = roleType.client;
      }
      if (sourceData.data) { 
        data = JSON.stringify(sourceData.data);
      } 

      console.log(role + " - 连接成功：", sourceData.type)
  
      connectReady[role] = sourceData.id
      connects[role] = conn
    } catch (e) {
      console.log("source 不是json结构化数据")
    }

    if (!ignoreType.includes(sourceData.type)) { 
      if (role === roleType.server) {
        if (sourceData.sendList) {
          sourceData.sendList.forEach(item => {
            send(role, item, data)
          })
        }
        else { 
          send(role, roleType.client, data) 
        }
      } else {
        send(role, roleType.server, data)
      }
    }
  })

  conn.on("close", (code, reason) => {
    console.log("服务关闭连接", code)
  });

  conn.on("error", (code, reason) => {
    console.log("服务异常关闭", code)
  });
}).listen(8005)

console.log("WebSocket建立完毕", 'ws://127.0.0.1:8005')
