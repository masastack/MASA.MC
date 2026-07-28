## Why

`GetSmsInteractionHistoryAsync` 返回短信互动历史时，当前结果按时间倒序排列。调用方需要按时间升序展示对话过程，从最早记录到最新记录阅读更符合互动历史的时间线语义。

## What Changes

- 将短信互动历史接口的最终结果排序从按 `SendTime` 降序改为升序。
- 保持现有查询范围、入站/出站合并、空内容过滤和 DTO 结构不变。
- 不修改 API 路由、请求参数、响应字段或数据库结构。

## Capabilities

### New Capabilities
- `sms-interaction-history`: 短信互动历史查询能力，包括入站短信与出站短信合并、内容过滤和结果排序。

### Modified Capabilities

## Impact

- 影响代码：
  - `src/Services/Masa.Mc.Service/Application/MessageRecords/MessageRecordQueryHandler.cs`
- 影响接口：
  - `MessageRecordService.GetSmsInteractionHistoryAsync`
- 不涉及 EF Core migration、Dapr 事件契约或 DTO 变更。
