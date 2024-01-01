# DotRedis(KestrelRedis)

随手写的 图一乐

基于.Net 8 Kestrel 服务器，偶然看到有人写了个 Netty 的 RedisSever，故萌生想法。

KestrelRedis 是基础版本的，没啥封装直接用的那种，好处是直接 aot。

KestrelRedisEncap 是封装较多版本的，参考了老九的文章去写，完善了基本功能，坏处是 aot 要自己写 rd.xml 了。

KestrelClient 用来测试客户端连通，没啥用（ redis-cli 可以直连反正~

东西尚属于初期，经 redis-benchmark 测试 SET GET DEL 操作仅略微落后于 Redis 本体，吞吐、总时间、延迟比看到的那个 Netty 写的强不少，如下图所示，环境为 wsl 最新版 ArchLinux。
java 出现 exception 原因未知，但这个 exception 控制台输出并不是无限刷的，只在测试开始会刷出来两次。都已经测试多遍 jit 预热，是最佳结果

c#:
![c#](https://github.com/mysteriousmy/DotRedis/blob/main/benchmark/c%23.png?raw=true)

java:
![java](https://github.com/mysteriousmy/DotRedis/blob/main/benchmark/java.png?raw=true)

其它数据结构暂未实现，后续抽空实现。
