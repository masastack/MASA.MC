# OpenSpec 工作流

本仓库使用 OpenSpec 做 SDD，也就是 spec-driven development。

当需求涉及新功能、行为变更、较大的重构、契约变化、数据库迁移、后台任务变化或消息发放语义变化时，应先创建 OpenSpec change。拼写修正、注释调整、纯机械清理等不改变需求的小改动，可以直接改代码。

## Commands

- `/opsx:explore "<问题或领域>"` - 需求或方案还不清楚时，先调研代码和现状。
- `/opsx:propose "<需求描述>"` - 在 `openspec/changes/<id>/` 下创建一次变更。
- `/opsx:apply <change-id>` - 按已确认的 tasks 实现变更。
- `/opsx:archive <change-id>` - 完成后把 delta specs 合并到 `openspec/specs/`。

官方 CLI 需要 Node.js 20.19.0 或更高版本。本仓库使用 `@fission-ai/openspec@1.6.0` 初始化，并配置给 Codex 使用。

## Change Shape

每个 change 通常包含：

- `proposal.md` - 为什么要做、要改什么、影响哪些能力。
- `specs/<capability>/spec.md` - ADDED/MODIFIED/REMOVED/RENAMED requirements。
- `design.md` - 当涉及架构、数据、依赖、部署或风险时，说明技术方案。
- `tasks.md` - 可跟踪的 checkbox 实施计划。

命名保持英文 kebab-case，例如 `message-template-rendering`、`sms-unsubscription`、`message-task-execution` 或 `website-message-delivery`。正文、业务说明和场景描述可以使用中文。
