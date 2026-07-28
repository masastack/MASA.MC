## 1. Implementation

- [x] 1.1 定位 `GetSmsInteractionHistoryAsync` 最终合并结果的排序逻辑。
- [x] 1.2 将最终排序从 `SendTime` 降序改为升序。
- [x] 1.3 确认入站/出站合并、空内容过滤、查询参数和 DTO 结构不变。

## 2. Validation

- [x] 2.1 运行 `openspec validate sort-sms-interaction-history-ascending`。
- [x] 2.2 运行受影响项目适用的 .NET build（当前被既有 `ChannelStatisticsQueryHandler` 中 `CultureTimeZoneResolver` 命名歧义阻塞）。
- [x] 2.3 确认不需要生成 EF Core migration。
