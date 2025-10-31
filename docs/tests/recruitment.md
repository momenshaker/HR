Recruitment Module – Test Plan

Scope
- Stage flow correctness
- Candidate email uniqueness
- Hire path (with optional Employee creation)

Stage Flow
- Applied → Screen → Interview → Offer → Hired
- Rejection allowed from any stage except Hired
- Invalid transitions return 422 with clear error

Test Cases
- Create job (valid/invalid payload)
- List jobs by org and status
- Update job; close job action sets status Closed
- Create application with existing candidateId
- Create application with new candidate payload
- Reject duplicate candidate email on application create (409)
- Advance: Applied→Screen (200)
- Advance: Screen→Interview (200)
- Advance: Interview→Offer (200)
- Advance: Offer→Hired with createEmployee=false (200)
- Advance: Offer→Hired with createEmployee=true (201/200) and verify employee creation side-effect
- Advance: Applied→Offer (422 invalid transition)
- Reject from Interview (200, stage=Rejected)

Interview
- Schedule interview with location (201)
- Schedule interview with meetingUrl (201)
- Reject schedule when neither location nor meetingUrl provided (400)
- Update interview feedback and outcome (200)

Validation & Errors
- Missing required fields → 400 ValidationError
- Not found IDs → 404 NotFound
- Duplicate candidate email → 409 Conflict
