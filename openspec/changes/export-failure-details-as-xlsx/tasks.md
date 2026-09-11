## 1. Excel 导出基础设施

- [x] 1.1 创建独立的 `Masa.Mc.Infrastructure.ExporterAndImporter.Excel` 项目
- [x] 1.2 在 Excel 项目中添加 `Magicodes.IE.Excel` 2.6.4 依赖
- [x] 1.3 在 Excel 项目中新增 XLSX exporter 接口、实现和 DI 注册，保留现有 CSV exporter
- [x] 1.4 使用默认自动列宽，并以部署镜像提供 `libgdiplus` 为运行环境前提
- [x] 1.5 验证字符串属性写入 XLSX 时为文本单元格，并覆盖长数字消息 ID

## 2. 失败详情导出链路

- [x] 2.1 将 `ChannelStatisticsQueryHandler` 的失败详情导出切换到 XLSX exporter
- [x] 2.2 保持现有筛选、排序、时区转换、空结果和 100,000 条上限行为
- [x] 2.3 将管理端下载文件名和 MIME 类型更新为 XLSX
- [x] 2.4 将预计发送时间和实际发送时间固定为 24 小时日期格式

## 3. 验证与交付

- [x] 3.1 增加或更新导出测试，验证长数字和非数字消息 ID 的完整文本值
- [x] 3.2 编译受影响的 Infrastructure、Service 和 Web 项目并修复编译错误
- [x] 3.3 对修改的 C# 和 Razor 文件执行 linter 检查，并确认 OpenSpec 状态完成
