## 1. Template Lookup Behavior

- [x] 1.1 识别 `MessageTaskCommandHandler` 和 `CreateTemplateMessageTaskCommandHandler` 中所有仅用于发放的模板读取点。
- [x] 1.2 将仅用于发放的 `MessageTemplate` 查询替换为 no-tracking 查询或保持只读语义的仓储 helper。
- [x] 1.3 保持真实模板编辑、审核、失效和删除流程中的 tracked 查询不变。

## 2. Delivery Flow Preservation

- [x] 2.1 更新内部模板消息发放，确保按模板编码读取模板时不会跟踪或修改模板。
- [x] 2.2 更新外部模板消息发放，确保按模板编码读取模板时不会跟踪或修改模板。
- [x] 2.3 更新 simple-send 模板消息发放，确保按模板编码读取模板时不会跟踪或修改模板。
- [x] 2.4 更新模板消息任务创建校验，确保读取模板展示名、签名、消息内容和站内信标记时不会跟踪或修改模板。
- [x] 2.5 更新模板发送事件处理器，确保发放期校验和渲染读取模板时不启用跟踪。

## 3. Verification

- [x] 3.1 验证仅用于发放的模板读取都使用 `FindNoTrackingAsync`，并确认仓库中没有可追加该场景的现成测试项目。
- [x] 3.2 验证模板不存在时仍然失败，且不会创建消息任务。
- [x] 3.3 验证模板实体 id、展示名、签名、消息类型和站内信标记等模板派生字段仍然正确填充。

## 4. Validation

- [x] 4.1 运行 `openspec validate fix-template-message-audit-fields`。
- [x] 4.2 运行受影响项目适用的 .NET build 或测试命令。
- [x] 4.3 确认不需要生成 EF Core migration。
