# AI Software Engineering Agent System — Project Specification

## 1. Mục tiêu

Xây dựng một hệ thống **AI Software Engineer / Multi-Agent Software Engineering System** có khả năng hỗ trợ xây dựng và phát triển một software project từ yêu cầu tự nhiên của người dùng.

Mục tiêu của MVP:

> Người dùng đưa một yêu cầu tính năng hoặc bug → hệ thống phân tích yêu cầu → lập kế hoạch → chỉnh sửa code → chạy test → tự sửa lỗi nếu cần → review → báo cáo kết quả.

Hệ thống cần ưu tiên tính kiểm soát, khả năng quan sát, khả năng đánh giá và an toàn khi agent thao tác trên source code.

---

## 2. Nguyên tắc kiến trúc

### 2.1 Không bắt đầu bằng quá nhiều agent

Triển khai theo từng tầng:

1. Tool layer
2. Single Coding Agent
3. Self-correction loop
4. Planner
5. Orchestrator
6. Multi-agent
7. Memory
8. Git/Sandbox
9. Evaluation
10. UI

Mục tiêu quan trọng nhất của MVP là làm cho **một Coding Agent thực sự có thể đọc, sửa và kiểm thử một project**.

### 2.2 Orchestrator là trung tâm

Các agent không nên tự do gọi nhau không kiểm soát.

Orchestrator quản lý workflow, state và chuyển task giữa các agent.

### 2.3 Agent và Tool phải tách biệt

Agent quyết định cần làm gì.

Tool thực hiện thao tác cụ thể.

Ví dụ:

- Agent: quyết định cần đọc `auth/service.py`.
- Tool: thực sự đọc file đó.

---

# 3. Kiến trúc tổng thể

```text
                         USER
                           |
                           v
                  +----------------+
                  | ORCHESTRATOR   |
                  +-------+--------+
                          |
             +------------+------------+
             |            |            |
             v            v            v
      Requirement      Planner      Project
         Agent          Agent       Memory
             |            |
             +------+-----+
                    |
                    v
              Coding Agent
                    |
          +---------+---------+
          |                   |
          v                   v
     File System            Git
          |
          v
       PROJECT
          |
          v
      Testing Agent
          |
       +--+--+
       |     |
     FAIL   PASS
       |     |
       v     v
    Coding  Reviewer
       ^      |
       |   +--+--+
       |   |     |
       +--FAIL  PASS
               |
               v
              DONE
```

---

# 4. Các agent

## 4.1 Orchestrator Agent

Trách nhiệm:

- Nhận task từ user.
- Quản lý workflow.
- Quản lý Project State.
- Quyết định agent nào được gọi tiếp theo.
- Xử lý PASS/FAIL.
- Giới hạn số vòng sửa lỗi.
- Không trực tiếp viết code.

## 4.2 Requirement Agent

Trách nhiệm:

- Phân tích yêu cầu tự nhiên.
- Làm rõ mục tiêu và phạm vi.
- Chuyển yêu cầu thành requirements có cấu trúc.
- Tạo functional/non-functional requirements khi cần.

Ví dụ:

```text
User:
"Tôi muốn user đăng nhập bằng Google."

Requirement Agent:

FR-001: User có thể chọn Login with Google.
FR-002: Hệ thống redirect tới Google OAuth.
FR-003: Hệ thống xử lý OAuth callback.
FR-004: Tạo user nếu chưa tồn tại.
FR-005: Tạo session sau authentication.
```

Requirement Agent không trực tiếp sửa source code.

## 4.3 Planner Agent

Trách nhiệm:

- Đọc requirements.
- Phân tích project hiện tại.
- Tạo implementation plan.
- Xác định file/module/API/database cần thay đổi.
- Xác định test cần thêm.

Ví dụ:

```json
{
  "task": "Add Google authentication",
  "steps": [
    "Inspect current authentication architecture",
    "Add Google OAuth configuration",
    "Add OAuth callback endpoint",
    "Create user mapping logic",
    "Update frontend login button",
    "Add tests",
    "Run test suite"
  ]
}
```

## 4.4 Coding Agent

Trách nhiệm:

- Đọc source code.
- Tìm code liên quan.
- Chỉnh sửa code.
- Tạo file khi cần.
- Chạy command cần thiết.
- Chạy test.
- Phân tích lỗi.
- Tự sửa trong giới hạn số vòng quy định.

Coding Agent là agent quan trọng nhất của MVP.

## 4.5 Testing Agent

Trách nhiệm:

- Chạy test.
- Chạy build/lint/type check nếu project hỗ trợ.
- Phân tích test failure.
- Tạo test khi cần.
- Trả về test report có cấu trúc.

Ví dụ:

```json
{
  "status": "FAIL",
  "passed": 42,
  "failed": 2,
  "errors": [
    {
      "test": "test_google_login",
      "message": "expected 200 but received 401"
    }
  ]
}
```

## 4.6 Reviewer Agent

Trách nhiệm:

- Review thay đổi.
- Kiểm tra correctness.
- Kiểm tra architecture.
- Kiểm tra security cơ bản.
- Kiểm tra coding conventions.
- Phát hiện regression.
- Trả về PASS/FAIL và danh sách issue.

Ví dụ:

```json
{
  "status": "FAIL",
  "issues": [
    {
      "severity": "HIGH",
      "file": "auth/service.py",
      "description": "JWT expiration is not validated"
    }
  ]
}
```

---

# 5. Tool Layer

Agent cần các tool tối thiểu sau.

## 5.1 File tools

```text
list_files()
read_file(path)
search_code(query)
write_file(path, content)
edit_file(path, ...)
```

Không nên cho agent thao tác ngoài workspace được phép.

## 5.2 Terminal tools

```text
run_command(command)
run_tests()
```

Terminal cần có sandbox/permission và giới hạn command nguy hiểm.

## 5.3 Git tools

```text
git_status()
git_diff()
git_checkout_branch()
git_commit()
git_reset()
```

Agent nên làm việc trên branch/task riêng.

---

# 6. Project State

Workflow phải có state có cấu trúc.

Ví dụ:

```python
class ProjectState:
    task: str
    requirements: list
    plan: list
    changed_files: list
    test_results: dict
    review_result: dict
    status: str
```

Có thể mở rộng:

```text
project_id
task_id
current_phase
current_agent
iteration
workspace
branch
requirements
plan
changed_files
test_results
review_result
errors
agent_trace
```

---

# 7. Workflow / State Machine

Workflow đề xuất:

```text
NEW
 |
 v
ANALYZING
 |
 v
PLANNING
 |
 v
IMPLEMENTING
 |
 v
TESTING
 |
 +---- FAIL ----> IMPLEMENTING
 |
 PASS
 |
 v
REVIEWING
 |
 +---- FAIL ----> IMPLEMENTING
 |
 PASS
 |
 v
DONE
```

Cần giới hạn số vòng retry/repair để tránh loop vô hạn.

Ví dụ:

```text
MAX_REPAIR_ITERATIONS = 5
```

Nếu vượt giới hạn:

```text
FAILED_REQUIRING_HUMAN_REVIEW
```

---

# 8. Agentic Coding Loop

Coding Agent cần có vòng lặp:

```text
Understand
   |
Inspect
   |
Plan
   |
Modify
   |
Test
   |
 +--+--+
 |     |
FAIL  PASS
 |     |
 v     v
Analyze Review/Done
 |
 v
Modify again
```

Ví dụ:

```text
1. Inspect project.
2. Find authentication code.
3. Read relevant files.
4. Identify bug.
5. Modify code.
6. Run tests.
7. Read failure.
8. Fix code.
9. Run tests again.
10. Stop when tests pass or retry limit is reached.
```

---

# 9. Memory

Memory nên chia thành các loại.

## 9.1 Short-term memory

Context của task hiện tại:

```text
User request
→ Requirements
→ Plan
→ Code changes
→ Test results
→ Review
```

## 9.2 Project memory

Thông tin tồn tại xuyên suốt project:

```text
Architecture
Technology stack
Database schema
Business rules
Coding conventions
Important decisions
Authentication mechanism
```

Ví dụ:

```json
{
  "architecture": {
    "frontend": "React",
    "backend": "FastAPI",
    "database": "PostgreSQL"
  },
  "authentication": "JWT"
}
```

## 9.3 Knowledge base

Có thể dùng RAG/vector database cho:

- Project specification.
- Documentation.
- Research papers.
- Framework documentation.
- API documentation.
- PDF/tài liệu liên quan.

---

# 10. Git và Workspace Isolation

Agent không nên trực tiếp sửa production/main branch.

Workflow đề xuất:

```text
Create task
   |
   v
Create isolated branch/workspace
   |
   v
Agent modifies code
   |
   v
git diff
   |
   v
Run tests
   |
   v
Review
   |
   +---- FAIL -> Fix
   |
   PASS
   |
   v
Commit
```

Cần có khả năng rollback.

---

# 11. Permission và Security

Mỗi agent phải có quyền tối thiểu cần thiết.

Ví dụ:

```text
Requirement Agent:
    READ project
    WRITE requirements

Planner Agent:
    READ project
    WRITE plan

Coding Agent:
    READ project
    WRITE source code
    EXECUTE approved commands

Testing Agent:
    READ project
    EXECUTE tests
    WRITE test reports

Reviewer Agent:
    READ project
    WRITE review reports
```

Các thao tác rủi ro cần permission riêng:

```text
run arbitrary command
git push
database migration
deployment
delete files
access outside workspace
```

Không cho agent mặc định có toàn quyền hệ thống.

---

# 12. Evaluation

Đây là phần quan trọng đối với Graduation Project.

Không chỉ demo rằng agent "có vẻ hoạt động".

Cần metrics.

## Task Success

```text
success rate =
successful tasks / total tasks
```

## Test Success

```text
test pass rate
```

## Repair Success

```text
repair success rate =
bugs successfully fixed / bugs attempted
```

## Code Quality

Có thể đánh giá:

- Test coverage.
- Lint errors.
- Type errors.
- Static analysis.
- Reviewer score.
- Human evaluation.

## Efficiency

Đo:

- Number of agent iterations.
- Number of tool calls.
- Execution time.
- Token/cost nếu cần.

---

# 13. Agent Builder / Meta-Agent (giai đoạn nâng cao)

Một hướng mở rộng là xây một **Agent Builder / Agent Factory**.

Mục tiêu:

> AI có thể thiết kế và tạo specialized agent dựa trên specification.

Workflow:

```text
Agent Builder
     |
     v
Analyze Agent Requirements
     |
     v
Design Agent
     |
     v
Generate Agent Code
     |
     v
Generate Tools
     |
     v
Generate Tests
     |
     v
Validate
     |
     v
Register Agent
```

Ví dụ:

```text
Input:
"Create a Testing Agent for Python FastAPI projects."

Output:
- testing_agent.py
- required tools
- prompts
- tests
- configuration
- permissions
- registration metadata
```

Đây là hướng nâng cao, không nên là MVP đầu tiên.

---

# 14. Đề xuất Tech Stack

Nếu dùng Python:

```text
Language:
Python

LLM:
Một LLM provider hỗ trợ tool/function calling

Agent orchestration:
LangGraph hoặc OpenAI Agents SDK

Backend:
FastAPI

Database:
PostgreSQL

Vector storage:
pgvector hoặc vector database tương đương

Source control:
Git

Testing:
pytest + project-specific test runner

Frontend:
Có thể làm sau bằng React/Next.js hoặc UI đơn giản trước
```

Không cần phụ thuộc framework agent quá sớm. Có thể tự xây abstraction layer để agent logic không bị khóa vào một framework.

---

# 15. Repository Structure

Đề xuất:

```text
ai-software-engineer/
|
├── agents/
│   ├── orchestrator.py
│   ├── requirement_agent.py
│   ├── planner_agent.py
│   ├── coder_agent.py
│   ├── tester_agent.py
│   └── reviewer_agent.py
|
├── tools/
│   ├── filesystem.py
│   ├── terminal.py
│   ├── git.py
│   └── search.py
|
├── workflow/
│   ├── state.py
│   └── graph.py
|
├── memory/
│   ├── project_memory.py
│   └── vector_store.py
|
├── evaluation/
│   ├── evaluator.py
│   └── metrics.py
|
├── api/
│   └── server.py
|
├── database/
│   └── models.py
|
├── config/
│   └── settings.py
|
├── tests/
|
├── main.py
└── requirements.txt
```

---

# 16. Implementation Roadmap

## Milestone 1 — LLM + Tool Calling

Mục tiêu:

```text
LLM
+
Tools
```

Agent có thể gọi tool.

## Milestone 2 — File Tools

Implement:

```text
list_files
read_file
search_code
write_file
edit_file
```

## Milestone 3 — Terminal

Implement:

```text
run_command
run_tests
```

## Milestone 4 — Single Coding Agent

Mục tiêu:

```text
Read
→ Analyze
→ Modify
→ Test
→ Fix
→ Test
```

Đây là MVP quan trọng nhất.

## Milestone 5 — Planner

```text
Requirement
→ Plan
→ Coding
```

## Milestone 6 — Multi-Agent

```text
Requirement Agent
→ Planner Agent
→ Coding Agent
→ Testing Agent
```

## Milestone 7 — Reviewer

```text
Testing
→ Review
→ Fix
```

## Milestone 8 — Memory

```text
PostgreSQL
+
Vector storage/RAG
```

## Milestone 9 — Git/Sandbox

```text
Branch
→ Agent changes
→ Test
→ Review
→ Commit
```

## Milestone 10 — Evaluation + UI

Xây dashboard và hệ thống benchmark/evaluation.

---

# 17. AI-Assisted Development

AI có thể được dùng để **xây chính hệ thống agents**.

Quy trình:

```text
Human defines architecture
        |
        v
AI generates implementation
        |
        v
Run code
        |
        v
Run tests
        |
        v
Human/AI review
        |
        v
Fix
        |
        v
Commit
```

AI phù hợp để hỗ trợ:

- Boilerplate.
- Agent implementation.
- Tool implementation.
- Test generation.
- Refactoring.
- Debugging.
- Documentation.
- Code review.

Con người vẫn phải kiểm soát:

- Architecture.
- Permissions.
- Security.
- Data model.
- Evaluation criteria.
- System boundaries.

---

# 18. Immediate Next Step

Không bắt đầu bằng frontend hoặc 6 agents.

Bắt đầu bằng một MVP rất nhỏ:

```text
Python project
    |
    v
LLM
    |
    v
Coding Agent
    |
    +-- list_files()
    +-- read_file()
    +-- search_code()
    +-- edit_file()
    +-- run_command()
    +-- run_tests()
    |
    v
Real Project
```

Mục tiêu đầu tiên:

> Cho agent một task đơn giản trên một project test, để agent tự đọc source → chỉnh sửa → chạy test → sửa lỗi → hoàn thành task.

Khi điều này hoạt động ổn định, mới thêm Planner, Testing Agent, Reviewer và Orchestrator.

---

# 19. Nguyên tắc phát triển

1. **MVP trước, multi-agent sau.**
2. **Tool layer phải rõ ràng và có permission.**
3. **Agent phải có state có cấu trúc.**
4. **Workflow phải giới hạn retry.**
5. **Mọi thay đổi code phải có Git/diff/rollback.**
6. **Không cho agent quyền hệ thống không cần thiết.**
7. **Mọi agent cần có trách nhiệm rõ ràng.**
8. **Evaluation phải được thiết kế từ sớm.**
9. **Ưu tiên khả năng quan sát: logs, traces, tool calls, state transitions.**
10. **Không đánh giá hệ thống chỉ bằng một demo thành công.**

---

# 20. Mục tiêu cuối cùng

Hệ thống hoàn chỉnh hướng tới:

```text
                    USER
                     |
                     v
              SOFTWARE REQUEST
                     |
                     v
              AI ORCHESTRATOR
                     |
        +------------+------------+
        |            |            |
        v            v            v
   REQUIREMENT    PLANNER      MEMORY
        |            |
        +------+-----+
               |
               v
          CODING AGENT
               |
               v
           TESTING
               |
          +----+----+
          |         |
        FAIL       PASS
          |         |
          v         v
        CODING    REVIEW
                    |
               +----+----+
               |         |
             FAIL       PASS
               |         |
               v         v
             CODING     DONE
```

Mục tiêu cuối cùng không phải chỉ là **"một chatbot biết code"**, mà là một **hệ thống AI Software Engineering có workflow, tools, memory, sandbox, multi-agent collaboration và evaluation rõ ràng**.
