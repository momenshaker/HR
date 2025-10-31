RecruitmentService – Design

Responsibilities
- Job lifecycle: create, update, close
- Application lifecycle: create (existing/new candidate), stage progression
- Interview: schedule and update (feedback/outcome)
- Hire path: on Offer→Hired, optionally create Employee

Stage Rules
- Allowed: Applied→Screen→Interview→Offer→Hired
- Rejected allowed from any stage except Hired
- All other transitions invalid (422)

Interfaces (illustrative)
- createJob(jobCreate): Job
- updateJob(id, jobUpdate): Job
- closeJob(id): Job
- createApplication(appCreate): Application
- advanceApplication(id, nextStage, createEmployee=false): Application
- scheduleInterview(interviewCreate): Interview
- updateInterview(id, interviewUpdate): Interview

Constraints
- Candidate email unique (enforced on create via repository or DB unique index)
- Interview requires exactly one of (location, meetingUrl)
- Job status set to Closed only via close action

Side Effects
- On Offer→Hired with createEmployee=true, create Employee and link to Candidate
