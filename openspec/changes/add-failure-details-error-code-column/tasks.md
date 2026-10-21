## 1. 云MAS错误码中文对照

- [x] 1.1 将管理端 `YunMasFailureReasonMapper` 复制到 `Masa.Mc.Infrastructure.Sms/YunMas`，namespace 改为 `Masa.Mc.Infrastructure.Sms.YunMas`
- [x] 1.2 在映射器中新增 `TryGetDescription(string? code, out string description)`，保留原有 `GetFailureReasonDisplay` 行为

## 2. 失败详情导出链路

- [x] 2.1 `FailureReasonDetailExportItem` 新增 `ErrorCode` 属性，表头为“错误码”，位于“失败原因”之前
- [x] 2.2 `ChannelStatisticsQueryHandler` 导出投影增加云MAS渠道判定 `IsYunMasSms`
- [x] 2.3 新增私有静态拆列方法，云MAS行写入错误码与中文失败原因（未命中回退为原始错误码），非云MAS行错误码为空且失败原因保持不变，空值为 `Unknown`
- [x] 2.4 保持现有筛选、排序、时区转换、空结果和 100,000 条上限行为

## 3. 验证与交付

- [x] 3.1 本地用临时单元测试验证映射器（命中、大小写不敏感、未命中、空值）、导出 handler（云MAS命中/未命中/空值、阿里云短信、非短信渠道、多渠道混合）和 XLSX 表头顺序；`Masa.Mc.Service.Tests` 项目按要求不纳入仓库，测试代码未随变更提交
- [x] 3.2 编译受影响的 Infrastructure.Sms、Contracts.Admin 和 Service 项目
- [x] 3.3 对修改的 C# 文件执行 linter 检查，并执行 `openspec validate`
