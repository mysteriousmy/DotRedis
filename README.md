# DotRedis(KestrelRedis)

随手写的 图一乐

基于.Net 8 Kestrel 服务器，偶然看到有人写了个 Netty 的 RedisSever，故萌生想法。

东西尚属于初期，经 redis-benchmark 测试 SET GET DEL 操作仅略微落后于 Redis 本体，吞吐、总时间、延迟比看到的那个 Netty 写的强不少

其它数据结构暂未实现，后续抽空实现。
