## Context

渠道统计失败原因详情由 `ChannelStatisticsQueryHandler.ExportChannelFailureReasonDetailsAsync` 查询失败的 `MessageRecordQueryModel`，映射为 `FailureReasonDetailExportItem` 后通过 XLSX exporter 输出。云MAS回执处理（`MessageReceiptCommandHandler.ReceiveYunMasReceiptAsync`）把回执 `ErrorCode` 直接写入 `FailureReason`，缺失时写入“发送失败”，因此云MAS渠道导出的“失败原因”全是错误码。

管理端已有 `YunMasFailureReasonMapper`（位于 `masa.stack.mfe` 仓库的 `Masa.Mc.Web.Admin/Global`），用于在页面把云MAS错误码翻译为中文；它不在本仓库内，后端 Service 无法引用。

现有统计逻辑已通过 `Channel.Type == ChannelTypes.Sms && Channel.Provider == (int)SmsProviders.Aliyun` 区分阿里云短信渠道，云MAS渠道可用 `SmsProviders.YunMas` 以同样方式识别。

## Goals / Non-Goals

**Goals:**

- 导出文件同时提供云MAS原始错误码和中文失败原因。
- 非云MAS渠道的导出内容与现有保持一致，仅多一个空的“错误码”列。
- 复用已有字典内容，不重新整理错误码。

**Non-Goals:**

- 不修改 `MessageRecord.FailureReason` 的存储内容或回执写入逻辑。
- 不修改管理端页面、饼图或消息记录列表的失败原因展示。
- 不为非云MAS渠道（阿里云短信、邮件、App 推送等）新增错误码解析。
- 不引入前后端共享字典的新包或新项目。

## Decisions

### 在 Infrastructure.Sms 的 YunMas 目录保留一份字典

在 `Masa.Mc.Infrastructure.Sms/YunMas` 新增 `YunMasFailureReasonMapper`，字典内容复制自管理端同名类，namespace 为 `Masa.Mc.Infrastructure.Sms.YunMas`。保留原有 `GetFailureReasonDisplay(string? reason, bool useFailureReasonMapping)`，并新增 `TryGetDescription(string? code, out string description)`，供导出逻辑判断是否命中。该项目已被 Service 引用，且 `YunMas` 命名空间已在两个项目的全局 using 中。

备选方案是放入 `Masa.Mc.Contracts.Admin`，让管理端后续复用同一份字典以消除重复。该方案更利于去重，但会让短信厂商字典进入对外契约层，且管理端仓库当前的 Contracts 副本与本仓库并不同步，因此本次选择放在云MAS基础设施目录，前后端各保留一份。

### 在 Service 层按渠道拆分两列

导出查询的匿名投影增加 `IsYunMasSms`（`Channel.Type == ChannelTypes.Sms && Channel.Provider == (int)SmsProviders.YunMas`），写法与 `GetChannelFailureReasonOverviewAsync` 中的 `IsAliyunSmsChannel` 一致，可翻译为 SQL，不增加额外查询。内存中通过一个私有静态拆列方法构建导出项：

- 云MAS：`ErrorCode` 为 trim 后的原始错误码；`FailureReason` 为字典中文说明，未命中时回退为原始错误码。原始值为空时 `ErrorCode` 为空，`FailureReason` 为 `Unknown`。
- 非云MAS：`ErrorCode` 为空；`FailureReason` 沿用现有逻辑（空值为 `Unknown`）。

拆列逻辑作为 handler 内部的私有静态方法，不对外暴露。本变更不在仓库中保留自动化测试，实现阶段通过本地临时单元测试（mock `IMcQueryContext` 与内存异步查询调用导出 handler）验证各场景。

### 导出列顺序

`FailureReasonDetailExportItem` 新增 `ErrorCode` 属性，表头为“错误码”，属性声明位于 `FailureReason` 之前，保证导出列顺序为：消息标题、渠道用户标识、错误码、失败原因、预计发送时间、实际发送时间、消息ID。

## Risks / Trade-offs

- [字典重复] 前后端各保留一份，新增错误码需要两边同步 → 在 proposal 中记录；后续可让管理端改为引用后端字典以去重。
- [错误码格式变化] 字典使用精确匹配（大小写不敏感），厂商若返回带额外前缀或后缀的错误码会回退为原始值 → 回退行为保证内容不丢失，且与管理端页面表现一致。
- [导出列变化] 下游若依赖固定列位置解析导出文件，会因新增列而错位 → 该导出由管理端按钮人工下载使用，接受该变化。
- [文件体积] 云MAS中文说明部分条目较长，会增加文件体积 → 当前导出上限为 100,000 条，增长可控；列宽沿用库的默认自动列宽。

## Migration Plan

1. 发布包含新增字典类、导出列和拆列逻辑的后端版本；无需数据库 migration。
2. 管理端无需配合发布，导出接口路径、参数和响应类型不变。
3. 回滚时恢复后端版本即可，不涉及数据回滚。

## Open Questions

- 无。
