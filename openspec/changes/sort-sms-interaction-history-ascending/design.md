## Context

`MessageRecordService.GetSmsInteractionHistoryAsync` 发布 `GetSmsInteractionHistoryQuery`，实际查询逻辑位于 `MessageRecordQueryHandler.GetSmsInteractionHistoryAsync`。

该 handler 会查询最近一个月内同一手机号和短信渠道下的出站消息记录，再查询对应入站短信记录，合并后过滤空内容。目前最终结果使用 `OrderByDescending(x => x.SendTime)`，导致接口按时间倒序返回。

## Goals / Non-Goals

**Goals:**

- 将最终返回结果改为按 `SendTime` 升序。
- 保持入站和出站记录合并逻辑不变。
- 保持最近一个月查询窗口、空内容过滤和 DTO 结构不变。

**Non-Goals:**

- 不新增排序参数。
- 不调整分页、查询时间窗口或短信内容渲染逻辑。
- 不改变数据库结构或索引。

## Decisions

- 只修改最终合并结果的排序方向。
  - Rationale: 当前需求只要求接口返回按时间升序。最终排序发生在入站和出站结果合并后，改这里可以保证整体时间线正确。
  - Alternative considered: 分别对入站和出站查询排序。这样不能单独保证合并后的整体顺序，还会引入不必要的查询变化。

## Risks / Trade-offs

- [Risk] 前端或调用方如果已经按倒序做展示，显示顺序会变化。
  - Mitigation: 本次需求明确要求接口按时间升序，服务端排序契约以该要求为准。

## Migration Plan

不需要数据库迁移。发布方式是纯代码部署，回滚方式是恢复原来的降序排序。

## Open Questions

- 无。
