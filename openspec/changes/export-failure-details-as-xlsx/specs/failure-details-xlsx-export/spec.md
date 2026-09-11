## ADDED Requirements

### Requirement: Export failure details as XLSX

The system SHALL export channel failure reason details as a valid XLSX workbook when an administrator requests the existing failure-details export endpoint.

#### Scenario: Successful failure details export

- **WHEN** an administrator requests the failure-details export with valid filters
- **THEN** the system returns XLSX bytes containing the existing failure detail columns and records
- **AND** the management client downloads the file with an `.xlsx` extension and the XLSX MIME type

#### Scenario: Export result is empty

- **WHEN** valid filters match no failed message records
- **THEN** the system returns a valid XLSX workbook with the export headers and no data rows

### Requirement: Preserve message IDs as text

The system SHALL write every exported message ID as a text cell containing the original string value, regardless of whether the value consists only of digits or exceeds 15 characters.

#### Scenario: Long numeric message ID

- **WHEN** a failed message record has a numeric-looking message ID longer than 15 characters
- **THEN** the generated XLSX cell contains the complete original message ID
- **AND** the cell type is text rather than numeric
- **AND** opening the workbook in Excel does not display the value in scientific notation

#### Scenario: Non-numeric message ID

- **WHEN** a failed message record has an alphanumeric or provider-specific message ID
- **THEN** the generated XLSX cell contains the exact original string value

### Requirement: Use a consistent date-time format

The system SHALL format the expected send time and actual send time columns as `yyyy-MM-dd HH:mm:ss` using a 24-hour clock.

#### Scenario: Export records with send times

- **WHEN** a failed message record contains an expected send time or actual send time
- **THEN** its corresponding XLSX cell uses the `yyyy-MM-dd HH:mm:ss` format
- **AND** the cell display does not include a weekday or AM/PM marker

### Requirement: Support automatic column sizing

The system SHALL generate failure-details workbooks with automatic column sizing when the Linux service image provides the required `libgdiplus` dependency.

#### Scenario: Export from Linux container

- **WHEN** an administrator requests a failure-details export from the Linux service container
- **THEN** the system returns the XLSX workbook with automatically sized columns
- **AND** the service image provides `libgdiplus`

### Requirement: Preserve existing export constraints

The system SHALL preserve the existing failure-details filters, descending send-time ordering, local-time conversion, and maximum export count validation when changing the file format.

#### Scenario: Export exceeds the maximum count

- **WHEN** the filtered failed records exceed 100,000
- **THEN** the system rejects the export with the existing user-facing oversized-export error
- **AND** the system does not generate a downloadable workbook
