## Context

模板消息发放流程会读取 `MessageTemplate` 实体，用来创建消息任务、构造 simple-send 事件，或在发送事件处理器里做限额、退订、变量转换和内容渲染。相关路径包括：

- `MessageTaskCommandHandler.SendTemplateMessageByInternalAsync`
- `MessageTaskCommandHandler.SendTemplateMessageByExternalAsync`
- `MessageTaskCommandHandler.SendSimpleMessageAsync`
- `CreateTemplateMessageTaskCommandHandler.CheckMessageTemplateAsync`
- `Application/MessageTasks/EventHandler` 下的模板发送事件处理器

`IMessageTemplateRepository.FindAsync` 默认返回 EF Core tracked 实体。这些流程只需要把模板作为只读输入，但 tracked 的审计聚合可能跟消息任务创建或消息记录保存处在同一个 Unit of Work 中，导致模板的修改人和修改时间被更新，即使模板本身没有被编辑。

## Goals / Non-Goals

**Goals:**

- 模板消息发放过程中的模板查询必须是只读的。
- 内部发放、外部发放、simple-send 和发送事件处理器不得更新模板的修改人和修改时间。
- 保持现有的模板存在性校验，以及从模板复制到任务/事件的数据行为。
- 不修改 API、DTO、事件契约、调度器行为和数据库结构。

**Non-Goals:**

- 不重构消息任务创建或发送事件处理机制。
- 不改变模板编辑、审核、失效、退订配置等语义。
- 不修复历史上已经被污染的模板审计数据。

## Decisions

- 为模板仓储增加显式的 no-tracking 查询方法。
  - Rationale: `FindNoTrackingAsync` 可以让调用点明确表达“只读发放查询”，同时不改变 `FindAsync` 对更新流程的既有 tracked 语义。
  - Alternative considered: 在每个调用点直接使用 `AsNoTracking()`。这种方式可行，但会重复查询细节，且 include 行为更容易不一致。

- 发放期模板查询统一使用 no-tracking。
  - Rationale: 发放只读取模板元数据。`AsNoTracking` 可以避免 EF Core 把模板聚合纳入创建任务、保存记录或发布发送事件的写入工作单元。
  - Alternative considered: 查询后手动 detach 实体。这个方式更脆弱，因为未来代码可能在 detach 前访问 tracked 实体，而且调用语义不够清楚。

- 校验逻辑继续保留在应用层 handler 中。
  - Rationale: 现有 handler 已经负责模板不存在等校验。本次修复只改变查询跟踪行为，不移动业务校验边界。
  - Alternative considered: 新增领域服务方法统一做发放查询。对这个窄修复来说过重。

- 保持 `FindAsync(..., include: false)` 为 tracked 查询。
  - Rationale: 现有调用方可能依赖 tracked 实体进行更新。新的 helper 是显式 opt-in，只用于只读发放行为。
  - Alternative considered: 把 `FindAsync(..., include: false)` 改成 no-tracking。这样会扩大行为变化面。

## Risks / Trade-offs

- [Risk] no-tracking 查询可能漏掉以前 `FindAsync` include 的导航数据。
  - Mitigation: 只在发放路径使用 no-tracking，并保留 `include` 参数；需要 `Items` 的路径继续使用 include，需要标量/owned 数据的路径可以不 include。

- [Risk] 未来新增发放 handler 时重新引入 tracked 模板读取。
  - Mitigation: 在 OpenSpec capability 中记录要求，并通过 `FindNoTrackingAsync` 让只读查询模式更容易被发现。

- [Risk] 历史模板审计字段已经被错误更新。
  - Mitigation: 本次只阻止未来继续污染；历史数据修复另起需求处理。

## Migration Plan

不需要数据库迁移。

发布方式是纯代码部署。正常构建和验证通过后即可部署。回滚方式是恢复旧查询逻辑，但那会重新引入模板审计字段被误更新的问题。

## Open Questions

- 是否需要修复历史上已经被发放流程误更新的模板审计数据？本次变更不包含该工作。
