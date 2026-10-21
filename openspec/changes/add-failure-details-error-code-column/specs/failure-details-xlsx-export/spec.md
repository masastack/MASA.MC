## ADDED Requirements

### Requirement: Export error code column for failure details

The system SHALL include an error code column immediately before the failure reason column in the channel failure-details XLSX export, so that the export columns are, in order: message title, channel user identity, error code, failure reason, expected send time, actual send time, and message ID.

#### Scenario: Export column order

- **WHEN** an administrator exports failure details with valid filters
- **THEN** the workbook headers are, in order, 消息标题, 渠道用户标识, 错误码, 失败原因, 预计发送时间, 实际发送时间, 消息ID

#### Scenario: Export result is empty

- **WHEN** valid filters match no failed message records
- **THEN** the system returns a valid XLSX workbook that still contains the error code column header and no data rows

### Requirement: Provide Chinese failure reason for YunMas error codes

The system SHALL, for failed records sent through an SMS channel whose provider is YunMas, write the original trimmed error code to the error code column and write the Chinese description mapped from that error code to the failure reason column. Error code matching SHALL be case-insensitive.

#### Scenario: YunMas error code has a Chinese description

- **WHEN** a failed record belongs to a YunMas SMS channel and its failure reason is an error code that exists in the YunMas error code mapping
- **THEN** the error code column contains the original error code
- **AND** the failure reason column contains the mapped Chinese description

#### Scenario: YunMas error code has no Chinese description

- **WHEN** a failed record belongs to a YunMas SMS channel and its failure reason is not present in the YunMas error code mapping
- **THEN** the error code column contains the original error code
- **AND** the failure reason column contains the same original error code

#### Scenario: YunMas record has an empty failure reason

- **WHEN** a failed record belongs to a YunMas SMS channel and its failure reason is empty or whitespace
- **THEN** the error code column is empty
- **AND** the failure reason column contains `Unknown`

### Requirement: Keep failure reason unchanged for non-YunMas channels

The system SHALL leave the error code column empty and keep the existing failure reason content for failed records that do not belong to a YunMas SMS channel.

#### Scenario: Aliyun SMS record

- **WHEN** a failed record belongs to an Aliyun SMS channel
- **THEN** the error code column is empty
- **AND** the failure reason column contains the original failure reason

#### Scenario: Non-SMS record

- **WHEN** a failed record belongs to a non-SMS channel such as email or app push
- **THEN** the error code column is empty
- **AND** the failure reason column contains the original failure reason

#### Scenario: Non-YunMas record has an empty failure reason

- **WHEN** a failed record does not belong to a YunMas SMS channel and its failure reason is empty or whitespace
- **THEN** the error code column is empty
- **AND** the failure reason column contains `Unknown`

### Requirement: Resolve YunMas channel per record

The system SHALL decide whether to apply the YunMas error code mapping based on the channel of each exported record, so that an export covering multiple channels applies the mapping only to the YunMas records.

#### Scenario: Export without channel filter

- **WHEN** an administrator exports failure details without a channel filter and the result contains YunMas and non-YunMas records
- **THEN** only the YunMas records have a non-empty error code column and a mapped failure reason
- **AND** the non-YunMas records keep their original failure reason with an empty error code column
