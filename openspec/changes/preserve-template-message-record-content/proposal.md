## Why

模板消息记录详情当前使用记录变量重新渲染当前模板，模板修改或删除后，历史记录展示的内容会与发送时不一致。需要在消息记录聚合中保留本次发送使用的内容快照，使详情、短信互动历史和退订时间线可靠反映历史内容。

## What Changes

- 为模板消息记录保存按接收人渲染后的 `MessageContent` 快照，并作为 `MessageRecord` 聚合的可选值对象管理。
- 新增 `MessageRecordContents` 持久化表，只生成 PostgreSQL migration，不回填历史记录。
- 普通消息仍从 `MessageInfo` 读取内容，不重复保存快照。
- 重试仍使用当前模板，并用本次重试内容覆盖原记录快照。
- 消息记录详情接口统一返回已解析的内容、内容来源和可靠性标记；前端不再二次查询普通消息或当前模板。
- 旧模板记录在无快照时回退当前模板解析并标记为不可靠；源内容不存在时返回不可用状态。

## Capabilities

### New Capabilities
- `message-record-content-snapshot`: 定义模板消息记录内容快照、重试更新、历史回退和统一详情查询行为。

### Modified Capabilities


## Impact

- 影响 Domain 层的 `MessageRecord` 聚合及新增的内容快照领域服务。
- 影响模板消息任务发送、simple-send、短信自动回复、各渠道处理器与消息重试处理器。
- 影响 Admin 消息记录详情契约、Caller SDK 和管理端详情页。
- 影响短信互动历史和退订时间线的外发内容解析。
- 新增 PostgreSQL migration；本变更不生成 SQL Server migration，不引入新的外部依赖。
