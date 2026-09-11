## Context

渠道统计失败原因详情由 `ChannelStatisticsQueryHandler` 查询后通过共享 CSV exporter 输出，管理端以 `.csv` 文件下载。`FailureReasonDetailExportItem.MessageId` 虽然是 `string`，但 CSV 不携带单元格类型，Excel 打开文件时会将纯数字长消息 ID 推断为数字。

本次变更跨越 Infrastructure、Service 和管理端 Web 三层，需要新增 `Magicodes.IE.Excel` 依赖，同时保持现有查询、筛选、数量限制和响应字节数组契约不变。

## Goals / Non-Goals

**Goals:**

- 为失败原因详情生成标准 XLSX 文件。
- 让消息 ID 以文本单元格写入，保留原始字符串和完整精度。
- 只改变该导出功能的文件格式和下载元数据。
- 与现有 `Magicodes.IE.Csv` 2.6.4 使用同一版本线。

**Non-Goals:**

- 不修改 `MessageRecord.MessageId` 的领域或数据库类型。
- 不改变失败记录筛选、排序、时区转换、导出上限和列内容。
- 不将所有现有 CSV 导出迁移为 XLSX。
- 不改变消息发送、回执、退订或后台任务逻辑。

## Decisions

### 使用 Magicodes.IE.Excel 生成 XLSX

在独立的 `Masa.Mc.Infrastructure.ExporterAndImporter.Excel` 基础设施项目中增加 Excel exporter，使用 `Magicodes.IE.Excel` 2.6.4，并沿用当前 exporter 的集合到 `byte[]` 调用模式。XLSX 单元格具有明确类型，`string` 属性会写为文本，不依赖 Excel 对 CSV 内容的猜测。

备选方案是继续使用 CSV 并在消息 ID 前添加单引号或公式文本标记。该方案改动较小，但会改变原始 CSV 字段内容，且对非 Excel 消费者不透明，因此不采用。

### 仅切换失败详情导出链路

为 Excel exporter 建立独立项目、接口和 DI 注册；`ICsvExporter` 与 CSV 项目保持只负责 CSV 功能。`ChannelStatisticsQueryHandler` 仅注入并调用 Excel exporter，导出模型继续使用 `FailureReasonDetailExportItem`，避免污染领域模型和通用 CSV 行为。

### 保持 API 响应契约不变

后端路由、查询参数和返回类型仍保持不变，仅返回 XLSX 字节。管理端将下载文件名改为 `.xlsx`，Content-Type 改为 `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`。

### 禁用失败详情导出的自动列宽

`Magicodes.IE.Excel` 2.6.4 的自动列宽会通过 EPPlus 调用 `System.Drawing`。由于 Service 运行在 Linux 容器中，失败详情导出模型必须同时设置 `AutoFitAllColumn = false`，并将每个 `ExporterHeader.IsAutoFit` 设为 `false`。前者关闭整表自动列宽，后者避免库在样式阶段对单列再次调用 `AutoFit()`。列宽由 Excel 默认值处理，不影响单元格文本类型和内容。

## Risks / Trade-offs

- [依赖兼容性] `Magicodes.IE.Excel` 可能引入额外 OpenXML 依赖或与当前包版本存在 API 差异 → 固定为 2.6.4，并通过还原、编译和实际字节流测试验证。
- [文件体积] XLSX 通常比 CSV 产生更多元数据 → 当前导出上限为 100,000 条，接受可控的体积增长。
- [客户端兼容性] 旧的 CSV 下载脚本可能依赖 `.csv` 扩展名 → 该接口是管理端按钮调用，随前端同步更新；接口路径和参数不变。
- [文本类型回归] 某些 Excel exporter 配置可能根据值推断类型 → 测试使用超过 15 位的纯数字消息 ID，读取生成的 XLSX 单元格确认其类型和完整值。
- [列宽体验] 禁用自动列宽后，部分列可能需要用户手动调整宽度 → 优先保证 Linux 服务端稳定导出；后续可使用不依赖 GDI 的固定列宽方案优化。

## Migration Plan

1. 发布包含新依赖、Excel exporter、后端调用和前端下载元数据的版本。
2. 用户继续通过同一“导出详情”按钮下载文件，文件扩展名变为 `.xlsx`。
3. 若需回滚，恢复前端 `.csv` 元数据及后端 `ICsvExporter` 调用即可；不涉及数据库回滚。

## Open Questions

- 无。导出列沿用现有列集合，消息 ID 的文本语义由 XLSX 单元格类型保证。
