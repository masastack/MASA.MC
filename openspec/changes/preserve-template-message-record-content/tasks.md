## 1. Domain Model

- [x] 1.1 Extend the MessageRecord aggregate with nullable MessageContent snapshot state and rich capture/refresh behaviors
- [x] 1.2 Add a domain service that builds recipient-specific template content snapshots, including SMS presentation rules
- [x] 1.3 Move template variable mapping into the MessageTemplate aggregate and remove the domain-service-to-domain-service dependency

## 2. Persistence And Contracts

- [x] 2.1 Map the optional owned snapshot to MessageRecordContents in write and query EF Core models
- [x] 2.2 Add message-record detail content DTOs, content-source enum, mappings, and caller contract changes
- [x] 2.3 Generate and review the PostgreSQL migration and model snapshot without adding a SQL Server migration

## 3. Sending And Retry

- [x] 3.1 Capture snapshots in task-based template send handlers for all supported channels
- [x] 3.2 Capture snapshots in simple-send, SMS auto-reply, and deferred website-message paths
- [x] 3.3 Refresh snapshots from the current template in all supported retry handlers

## 4. Query Consumers And Frontend

- [x] 4.1 Resolve ordinary content, stored snapshots, legacy template fallback, and unavailable content in message-record detail queries
- [x] 4.2 Prefer stored snapshots in SMS record/history and unsubscription timeline content resolution
- [x] 4.3 Update the admin message-record detail UI to render the unified detail content and legacy reliability warning

## 5. Verification

- [x] 5.1 Add focused domain/query coverage for snapshot capture, replacement, fallback, and ordinary-message behavior
- [x] 5.2 Build and test the backend solution and frontend solution, and validate the OpenSpec change
