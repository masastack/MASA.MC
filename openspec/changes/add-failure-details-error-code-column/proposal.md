## Why

渠道统计的失败原因详情导出（XLSX）中，云MAS渠道的“失败原因”列写入的是回执原始错误码（例如 `CM:0142`、`DB:0141`），测试和运营人员无法直接读懂。管理端页面已经通过 `YunMasFailureReasonMapper` 把云MAS错误码翻译为中文，但导出文件没有同样的信息，需要在导出中同时提供错误码和中文失败原因。

## What Changes

- 失败原因详情导出新增“错误码”列，位于“失败原因”列之前。
- 云MAS短信渠道的记录：“错误码”列写入原始错误码，“失败原因”列写入错误码对应的中文说明；字典中不存在该错误码时，“失败原因”列回退为原始错误码。
- 非云MAS渠道的记录：“错误码”列为空，“失败原因”列保持现有内容（原始失败原因，空值为 `Unknown`）。
- 在后端 `Masa.Mc.Infrastructure.Sms` 的 `YunMas` 目录新增 `YunMasFailureReasonMapper`，字典内容复制自管理端同名类，并提供是否命中的查询方法。
- 保持现有筛选条件、倒序排序、时区转换、预计/实际发送时间格式、消息 ID 文本语义、最大导出数量限制和 API 返回 `byte[]` 的方式不变。
- 不修改消息记录领域模型、数据库字段或消息发送、回执、退订、重试行为。

## Capabilities

### New Capabilities

无。

### Modified Capabilities

- `failure-details-xlsx-export`: 导出列集合变化，新增“错误码”列，并规定云MAS与非云MAS记录在“错误码”和“失败原因”两列中的内容。该 capability 由尚未 archive 的变更 `export-failure-details-as-xlsx` 引入，本变更使用 `ADDED Requirements` 追加需求，需在该变更 archive 之后再 archive。

## Impact

- 影响 `Masa.Mc.Infrastructure.Sms` 项目：新增云MAS错误码中文对照类。
- 影响 `Masa.Mc.Contracts.Admin` 的 `FailureReasonDetailExportItem`：新增 `ErrorCode` 导出列。
- 影响 `ChannelStatisticsQueryHandler` 的失败详情导出：投影增加云MAS渠道判定，并拆分错误码与失败原因列。
- 接口路径、查询参数和响应类型不变，管理端无需改动；导出文件的列数从 6 列变为 7 列。
- 管理端仓库（`masa.stack.mfe`）中的 `YunMasFailureReasonMapper` 保持不动，字典在前后端各保留一份，后续新增错误码需要两边同步。
- 不涉及数据库 migration、Dapr、scheduler、消息投递、回执或退订行为。
