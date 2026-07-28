## ADDED Requirements

### Requirement: 短信互动历史按时间升序返回
系统 SHALL 按 `SendTime` 升序返回短信互动历史结果。

#### Scenario: 返回合并后的短信互动历史
- **WHEN** 调用方查询指定手机号和短信渠道的互动历史
- **THEN** 系统返回入站和出站短信合并后的结果
- **AND** 结果按 `SendTime` 从早到晚排列

#### Scenario: 过滤空内容后排序
- **WHEN** 查询结果中存在内容为空或空白的记录
- **THEN** 系统先排除这些记录
- **AND** 剩余结果仍按 `SendTime` 从早到晚排列

### Requirement: 短信互动历史保持现有查询边界
系统 MUST 保持现有查询条件和响应结构不变。

#### Scenario: 查询参数不变
- **WHEN** 调用方使用现有 `mobile` 和 `channelId` 参数查询短信互动历史
- **THEN** 系统继续使用相同参数返回 `SmsInteractionHistoryDto` 列表
- **AND** 不要求调用方传入新的排序参数
