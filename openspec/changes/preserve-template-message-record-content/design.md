## Context

消息记录只保存内容实体类型、实体 id 和变量。管理端查看模板消息记录时，会重新读取当前模板并渲染；模板修改后历史内容随之变化。短信互动历史和退订时间线也有相同依赖。

发送处理器已在调用渠道发送器前获得按接收人渲染的 `MessageData` 或等价的短信内容，因此可以在发送记录聚合中捕获这一时点的内容。现有重试会更新同一条 `MessageRecord` 并使用当前模板，快照需要与这一语义保持一致。

## Goals / Non-Goals

**Goals:**

- 通过充血聚合行为捕获和更新模板消息内容快照。
- 让新模板消息记录不再依赖当前模板展示历史内容。
- 保持普通消息的 `MessageInfo` 存储和展示路径不变。
- 为旧记录提供显式标记的回退行为。

**Non-Goals:**

- 不建立模板版本管理。
- 不改变重试使用当前模板的行为。
- 不为普通消息重复保存内容。
- 不回填旧记录，不生成 SQL Server migration。

## Decisions

### 快照属于 MessageRecord 聚合

`MessageRecord.ContentSnapshot` 使用现有 `MessageContent` 值对象，以 optional owned entity 方式映射到 `MessageRecordContents`。快照没有独立仓储或应用服务，其生命周期由 `MessageRecord` 管理。

聚合提供捕获和重试更新方法，校验记录类型与重试状态，并深拷贝 `MessageContent` 及扩展属性。应用处理器不直接设置快照状态。

### 统一的领域服务生成快照

`MessageRecordContentDomainService` 使用领域对象和现有渲染抽象生成最终展示内容。模板变量映射是 `MessageTemplate` 聚合自身的业务规则，由聚合行为 `ConvertVariables(...)` 负责；快照领域服务不依赖其他领域服务。通用渠道使用按接收人渲染的 `MessageData.MessageContent`；短信根据 provider 占位符、映射变量、签名和退订尾缀生成可还原内容。展示相关选项写入 `MessageContent.ExtraProperties`，发送元数据继续留在 `MessageRecord.ExtraProperties`。

### 详情查询返回统一内容

详情查询使用独立的 `MessageRecordDetailDto`：普通消息读取 `MessageInfo`，模板消息优先读取快照，旧模板记录回退当前模板。`ContentSource` 和 `IsContentReliable` 使回退或不可用状态对前端可见。列表查询不加载大文本快照。

### 重试覆盖快照

重试保持现有的“更新同一条记录”语义，使用当前模板生成新快照并替换原快照。这保证详情与记录上最近一次发送结果一致。

### 仅生成 PostgreSQL migration

公共 EF Core 模型保持 provider-neutral，但本变更只向 PostgreSQL migration 项目添加 migration 和模型快照更新。SQL Server 部署在后续补齐 migration 之前不得启用此功能。

## Risks / Trade-offs

- [按接收人保存内容增加数据量] → 快照使用独立表，列表查询不 join，只在详情和需要历史正文时读取。
- [各渠道渲染时点不一致] → 所有发送和重试入口统一通过领域服务生成快照，渠道处理器只传入最终发送所需的领域数据。
- [旧记录无法还原真实历史] → 不伪造回填，回退时显式标记不可靠。
- [SQL Server 模型与数据库不同步] → 在发布说明中明确 SQL Server 不在本次可部署范围。

## Migration Plan

1. 部署前在 PostgreSQL 执行新 migration，创建可空的一对一快照表。
2. 部署后新模板发送自动产生快照，旧记录保持原样并由查询层回退。
3. 回滚应先回滚应用，再执行 migration Down 删除快照表；回滚会丢失新产生的快照，但不影响原 `MessageRecords` 和 `MessageInfos` 数据。

## Open Questions

无。
