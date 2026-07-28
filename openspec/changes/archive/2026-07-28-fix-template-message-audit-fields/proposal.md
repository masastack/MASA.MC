## Why

发放模板消息时，系统只需要读取被引用的消息模板元数据，但当前流程可能会顺带更新该模板的修改人和修改时间。这样会污染模板审计信息，也会让模板列表看起来像是模板被人工编辑过，实际只是被用于发放。

## What Changes

- 确保模板消息发放流程读取消息模板时，不会把模板标记为已修改。
- 内部发放、外部发放、simple-send 以及发送事件处理器只引用模板时，必须保留模板的修改人和修改时间。
- 保持现有校验行为：引用的模板不存在时，发放仍然必须失败。
- 不修改 API 契约、DTO、数据库结构或迁移。

## Capabilities

### New Capabilities
- `template-message-delivery`: 模板消息发放能力，包括模板查找、基于模板创建消息任务，以及发放过程中保护模板审计元数据。

### Modified Capabilities

## Impact

- 影响代码：
  - `src/Services/Masa.Mc.Service/Application/MessageTasks/MessageTaskCommandHandler.cs`
  - `src/Services/Masa.Mc.Service/Application/MessageTasks/CreateTemplateMessageTaskCommandHandler.cs`
  - `src/Services/Masa.Mc.Service/Application/MessageTasks/EventHandler/*`
  - `src/Domain/Masa.Mc.Domain/MessageTemplates/Repositories/IMessageTemplateRepository.cs`
  - `src/Infrastructure/Masa.Mc.EntityFrameworkCore/Repositories/MessageTemplateRepository.cs`
- 影响流程：
  - `SendTemplateMessageByInternalAsync`
  - `SendTemplateMessageByExternalAsync`
  - `SendSimpleMessageAsync`
  - 模板消息任务创建前的模板校验流程
  - 基于模板的发送事件处理器，这些处理器会读取模板做限额校验、退订校验、变量转换、渲染或组装供应商请求
- 预计不影响公开 DTO、路由、Dapr 事件契约、调度器行为或 EF Core 迁移。
