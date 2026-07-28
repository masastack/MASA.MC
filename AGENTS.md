# Repository Instructions

本仓库的非平凡产品或架构变更使用 OpenSpec。

- 当需求不清楚或需要先调研代码时，先使用 `/opsx:explore`。
- 新功能、需求行为变化、API/契约变化、数据库迁移、后台任务或消息发放行为变化，先使用 `/opsx:propose`。
- OpenSpec 变更放在 `openspec/changes/<change-id>/`。
- `openspec/specs/` 下的 capability specs 通过 OpenSpec archive 流程更新。
- 不改变需求的小修复可以直接实现。

SDD 文档风格：

- change-id、capability 名、目录名使用英文 kebab-case。
- OpenSpec 固定结构和关键字保持英文，例如 `## ADDED Requirements`、`### Requirement:`、`#### Scenario:`、`WHEN`、`THEN`、`AND`。
- 业务背景、需求正文、场景描述、设计说明和任务内容可以使用中文。

工程默认规则：

- 保持现有 .NET 6、MASA Framework、DDD、CQRS、EF Core、Dapr 和分层项目结构。
- 区分 Domain、Contracts、Infrastructure、Service 和 ApiGateway 的职责。
- 持久化变化要同时考虑 PostgreSQL 和 SQL Server migration 项目。
- 消息相关变化要考虑模板、接收人、退订、重试、回执、消息记录和后台任务行为。
- 优先使用现有抽象、验证器、扩展方法、仓储模式和包版本。
