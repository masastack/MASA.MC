## Why

渠道统计中的失败原因详情目前以 CSV 导出。CSV 不包含单元格类型，Excel 打开纯数字消息 ID 时会自动按数字解析，导致显示为科学计数法，超过 15 位时还可能发生精度丢失。需要改用支持单元格类型的 XLSX 导出，确保消息 ID 保持完整文本。

## What Changes

- 将渠道失败原因详情导出的文件格式从 CSV 改为 XLSX。
- 使用 `Magicodes.IE.Excel` 生成工作簿，并将消息 ID 作为文本单元格写入。
- 更新管理端下载文件名和 MIME 类型。
- 保持现有筛选条件、导出列、最大导出数量限制和 API 返回 `byte[]` 的方式不变。
- 不修改消息记录领域模型、数据库字段或消息发送行为。

## Capabilities

### New Capabilities

- `failure-details-xlsx-export`: 提供渠道失败原因详情的 XLSX 导出，并保证消息 ID 内容完整且按文本处理。

### Modified Capabilities

- 无。

## Impact

- 影响 `ChannelStatisticsQueryHandler` 的导出依赖和实现。
- 影响新增的 `Masa.Mc.Infrastructure.ExporterAndImporter.Excel` 基础设施项目，使用与现有 `Magicodes.IE.Csv` 2.6.4 匹配的 `Magicodes.IE.Excel` 2.6.4 依赖及 XLSX 导出注册。
- 影响管理端下载文件扩展名和 MIME 类型；接口路径、查询参数和响应类型保持不变。
- 不涉及数据库 migration、Dapr、scheduler、消息投递、回执或退订行为。
