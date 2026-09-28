# Nghiên cứu hệ thống hướng dẫn Agent của dự án NCKH

> Mục tiêu: giải mã cách repository này điều phối một coding agent, phân biệt rõ với việc “huấn luyện AI”, và đưa ra thiết kế có thể áp dụng cho một project khác.
>
> Phạm vi khảo sát: cấu hình và tài liệu hiện có tại thời điểm lập báo cáo. Các kết luận về **cơ chế** được rút từ file trong repository; không suy đoán về prompt hệ thống, quyền truy cập, model hoặc công cụ nằm ngoài repository.

## Tóm tắt điều hành

Chủ project không huấn luyện lại trọng số (weights) của mô hình. Họ xây dựng một **hệ thống điều hành công việc cho agent** bằng cách ghép năm lớp:

1. **Hiến pháp repository** trong `AGENTS.md`: các bất biến về kiến trúc, ngôn ngữ, UI, an toàn dữ liệu và cách làm việc.
2. **Skills có phạm vi hẹp** trong `.agents/skills/`: biến những công việc lặp lại thành thủ tục có đầu vào, đầu ra, điểm dừng và lệnh kiểm chứng.
3. **Quy trình BMAD có trạng thái** trong `_bmad/` và `_bmad-output/`: chuyển yêu cầu từ PRD → Epic → Story → Spec → mã nguồn → review; mọi bước có artifact lưu lại.
4. **Human-in-the-loop** qua Checkpoint 1 và Lavish: agent phải lập kế hoạch, người dùng duyệt trước, rồi mới được sửa mã cho các story dùng workflow này.
5. **Ràng buộc thực thi**: build/test, baseline commit, review đa lăng kính, quản lý worktree/cổng chạy và các script giảm thao tác thủ công.

Điểm đáng học nhất không phải là viết prompt dài. Đó là biến các quyết định quan trọng thành **dữ liệu bền vững, có thể đọc lại và có nơi kiểm chứng**. Một agent mới vào repository có thể lần theo các file để biết: sản phẩm phải làm gì, hạn chế ra sao, feature đang ở đâu, file nào sẽ đổi, ai đã duyệt, và bằng chứng nào chứng minh đã hoàn thành.

Mô hình khái quát:

```text
Yêu cầu của người dùng
        │
        ▼
AGENTS.md + Skill phù hợp + ngữ cảnh repository
        │
        ▼
PRD / UX / Architecture / Epic context / Sprint status
        │
        ▼
Spec có phạm vi đóng băng + kế hoạch trực quan Lavish
        │
        ├── Người dùng review, ghi chú, phê duyệt ──┐
        ▼                                           │
Baseline commit → triển khai → build/test → review  │
        │                                           │
        └──────────── artifacts, log, trạng thái ◄──┘
```

---

## 1. “Train agent” ở đây thực chất là gì?

### 1.1. Bốn khái niệm cần tách bạch

| Khái niệm | Có xảy ra trong project này? | Ý nghĩa thực tế |
|---|---:|---|
| Fine-tuning / train weights | Không thấy | Thay đổi tham số mô hình bằng bộ dữ liệu huấn luyện. Repository không thể tự làm điều này chỉ bằng `AGENTS.md`. |
| System prompt của nền tảng | Không quan sát được từ repo | Nền tảng agent quyết định luật an toàn, quyền tool, cách ưu tiên instruction. Chủ project thường không kiểm soát toàn bộ lớp này. |
| Repository instructions | Có | Hướng dẫn được nạp khi agent làm việc trong repo: chuẩn code, workflow, hạn chế và cách giao tiếp. `AGENTS.md` là lớp chủ đạo. |
| Skill / workflow / artifact | Có | “Playbook” theo tác vụ. Nó hướng dẫn agent phải đọc gì, tạo gì, kiểm chứng ra sao và dừng ở đâu. |

Nói chính xác hơn, đây là **context engineering** và **workflow engineering**, không phải machine learning training. Hiệu quả đạt được vì agent nhìn thấy đúng thông tin, vào đúng thời điểm, trong cấu trúc có thể kiểm chứng.

### 1.2. Vì sao cách này tạo cảm giác agent đã được “đào tạo”?

Một agent dùng cùng model nhưng làm trong hai repo khác nhau sẽ hành xử khác vì:

- Repository này đặt quy tắc cụ thể ngay tại nơi agent làm việc.
- Quy tắc gắn vào hành động: ví dụ Entity Domain không được chứa data annotation, chứ không chỉ nói “viết clean code”.
- Workflow bắt buộc tạo bằng chứng ngoài hội thoại: spec, trạng thái sprint, diff review, test.
- Một số rào chắn có từ khóa rõ ràng: chưa duyệt Checkpoint 1 thì không được code.
- Ngữ cảnh được chia nhỏ theo Story/Epic nên agent không phải tự suy ra toàn bộ kiến trúc trong một lần.

Những điều trên tăng xác suất agent làm đúng, nhưng **không bảo đảm tuyệt đối**. Agent vẫn có thể hiểu sai, bỏ sót, hay không thể thực hiện quy trình nếu nền tảng không cho phép gọi tool tương ứng. Vì vậy, các rào chắn quan trọng cần được hậu thuẫn thêm bằng test, CI/CD, quyền truy cập và review của con người.

---

## 2. Bản đồ các thành phần đã quan sát

| Vị trí | Vai trò | Tính chất | Giá trị mang lại |
|---|---|---|---|
| `AGENTS.md` | Hiến pháp làm việc trong repo | Team-shared, quy định tổng quát | Định nghĩa chuẩn kiến trúc, UI, tiếng Việt và Lavish. |
| `.agents/skills/bmad-build-story/SKILL.md` | Workflow triển khai User Story | Team-shared, chuyên biệt NCKH | Đặt Checkpoint 1, format spec, trình tự code/review. |
| `.agents/skills/lavish/SKILL.md` | Hướng dẫn dùng Lavish | Skill cài từ nguồn ngoài | Biến báo cáo/kế hoạch thành HTML có thể annotate. |
| `.agents/skills/run/SKILL.md` | Vận hành nhiều worktree | Team-shared | Quy tắc cổng riêng theo nhánh, build và chạy FE/BE. |
| `.agents/skills/run/scripts/run.ps1` | Tự động hóa workflow run | Executable | Tính cổng, chỉnh `launchSettings.json` và proxy FE. |
| `_bmad/config.toml` | Cấu hình BMAD dùng chung | Installer-managed | Tên project, output folder, vai trò agent mặc định. |
| `_bmad/config.user.toml` | Tuỳ chọn cá nhân | User-scoped | Tên người dùng, ngôn ngữ giao tiếp, cấp độ kỹ năng. |
| `_bmad/custom/config.toml` | Ghi đè dùng chung | Committed, bền qua cập nhật | Nơi đúng để thay đổi BMAD mà không sửa file generated. |
| `_bmad/scripts/*.py` | Resolve/render cấu hình | Công cụ hệ thống | Merge cấu hình theo tầng và tạo snapshot workflow bất biến. |
| `_bmad-output/planning-artifacts/` | Nguồn quyết định sản phẩm | Versioned artifacts | PRD, UX, epics, architecture, review planning. |
| `_bmad-output/implementation-artifacts/` | Trạng thái và bằng chứng giao hàng | Versioned artifacts | Sprint status, epic context, spec từng Story, review diff. |
| `.lavish/` | Artifact review trực quan | Generated/review artifact | HTML cho plan/report, có thể nhận feedback trong session Lavish. |
| `skills-lock.json` | Khoá nguồn skill | Dependency metadata | Ghi nguồn GitHub và hash của Lavish để truy xuất nguồn gốc. |

### 2.1. Phân tầng bền vững của cấu hình BMAD

Trong phần đầu `_bmad/config.toml`, installer đã ghi rõ file này sẽ bị tái sinh sau khi cài đặt/cập nhật. Hệ thống dành chỗ cho nhiều tầng cấu hình:

```text
Mặc định do BMAD cài đặt
        ↓ merge
_bmad/config.toml                 (team/base; installer-managed)
        ↓ merge
_bmad/config.user.toml            (cá nhân; installer-managed)
        ↓ merge
_bmad/custom/config.toml          (team override; commit vào Git)
        ↓ merge
_bmad/custom/config.user.toml     (cá nhân override; nên gitignore)
```

`resolve_config.py` và `config_utils.py` làm việc merge bốn tầng này. Đây là chi tiết kỹ thuật rất tốt để kế thừa: không sửa trực tiếp file sinh tự động; thay đổi của team và thay đổi riêng cá nhân được tách riêng nên upgrade công cụ ít gây mất cấu hình.

### 2.2. Personas BMAD không phải multi-agent runtime

`_bmad/config.toml` định nghĩa các vai trò Business Analyst, Product Manager, UX Designer, System Architect và Senior Software Engineer; kèm tên, chức danh, mô tả phong cách tư duy/giao tiếp. Đây là **persona descriptor** để skill hoặc giao diện BMAD dùng khi cần, không tự chứng minh rằng năm agent đang chạy song song.

Khi mang sang project khác, hãy coi persona là một cách thay đổi lens tư duy. Chỉ nên thêm multi-agent thực sự khi nền tảng có điều phối, giới hạn quyền và cơ chế tổng hợp kết quả; nếu không, một agent với checklist review độc lập thường đơn giản và đáng tin hơn.

---

## 3. `AGENTS.md`: hiến pháp repository

### 3.1. Vai trò

`AGENTS.md` là lớp chỉ dẫn dùng chung, có độ bao phủ toàn repo. Nó trả lời năm câu hỏi trước khi agent bắt đầu code:

1. Hệ thống gồm những phần nào?
2. Những chuẩn kỹ thuật nào không được phá vỡ?
3. Những hành vi UI/UX nào bị cấm?
4. Artifact và quy trình review phải đi qua đâu?
5. Cách dùng ngôn ngữ nào phù hợp với domain?

Đây là thiết kế tốt hơn một README chung chung vì các điều khoản có thể chuyển thẳng thành quyết định code.

### 3.2. Bất biến backend

Các rule backend được nêu rất cụ thể:

- `BE/src/Domain` giữ entity POCO thuần; cấm data annotation mapping schema như `StringLength`, `Required`, `Table`, `Column`.
- EF Core schema là single source of truth tại `Infrastructure/Persistence/Configurations/` qua Fluent API.
- Các trường audit và `DomainEvents` được cấu hình tập trung ở `ApplicationDbContext.OnModelCreating`.
- Validation nằm ở Application (FluentValidation) hoặc domain logic.

Đây là dạng policy có ích vì agent thường có khuynh hướng thêm `[Required]` hay `[MaxLength]` ngay vào entity. Quy tắc không chỉ cấm mà còn chỉ **điểm đặt thay thế**, nên giảm nguy cơ agent bị kẹt hoặc dùng một giải pháp tuỳ tiện.

### 3.3. Bất biến frontend

Các chỉ dẫn FE có tính domain và thiết kế mạnh:

- Ưu tiên component đã có của Velzon và các thư viện đã tích hợp (`@ng-select`, `flatpickr`, `@ng-bootstrap/ng-bootstrap`, `ngx-dropzone`, `sweetalert2`).
- Không dùng input native thô cho select/date nếu làm giảm nhất quán UI.
- Cấm banner giải thích dài, ghi chú quy tắc nghiệp vụ dư thừa trong UI.
- Bắt buộc dark mode/light mode, responsive, CSS variables/theme tokens; cấm hard-code màu nền/chữ.
- Cấm mock data FE; phải gọi API, tương tác người dùng thật hoặc seed ở backend/test.

Mức chi tiết này thể hiện một triết lý quan trọng: **agent cần quy tắc ở ngay nơi nó hay sai**. Nếu chỉ nói “UI đẹp, responsive”, kết quả không thể kiểm chứng. Còn “không mock data”, “chỉ dùng theme tokens” và “đưa test data về backend seed” là các quy tắc có thể kiểm bằng review hoặc lint.

### 3.4. Chuẩn ngôn ngữ như một domain constraint

Project bắt agent dùng văn phong hành chính tiếng Việt và có bảng thay thế thuật ngữ. Đây không phải chỉ là vấn đề copywriting: nó làm giao diện nhất quán với môi trường quản lý khoa học. Danh sách cấm/dùng thay thế giúp agent tránh các bản dịch máy và giữ định danh nghiệp vụ như `MocThoiGian`, `DotDangKy`, `DeTai`.

### 3.5. Điều gì nên cải thiện trong `AGENTS.md` khi học theo?

Không nên biến `AGENTS.md` thành nơi chứa mọi kiến thức. File hiện đã khá dài; nếu kéo dài vô hạn, agent có thể bỏ sót các rule quan trọng. Một thiết kế tốt hơn cho project mới là:

- Giữ `AGENTS.md` khoảng 150–300 dòng: nguyên tắc chung, cây thư mục, lệnh verify, policy an toàn, và link đến rule chi tiết.
- Dồn ràng buộc feature/domain vào spec hoặc `docs/architecture/`.
- Dùng script/lint/test để thực thi rule nguy hiểm; không dựa hoàn toàn vào văn bản.
- Tránh đường dẫn tuyệt đối tới máy của một người trong template dùng chung; ưu tiên đường dẫn tương đối trong repo.

---

## 4. Skill: biến tri thức thành thủ tục vận hành

### 4.1. Cấu trúc chung của một skill tốt

Mỗi `SKILL.md` nên trả lời theo thứ tự:

```text
Khi nào phải dùng skill này?
→ Phải đọc/nghiên cứu những nguồn nào trước?
→ Phải tạo hay sửa những artifact nào?
→ Các bước có thứ tự gì?
→ Điều kiện dừng hoặc chốt phê duyệt là gì?
→ Lệnh kiểm chứng nào phải chạy?
→ Handoff/output cần báo người dùng là gì?
```

Skill không phải một đoạn prompt quảng cáo năng lực. Nó là **runbook có thể thực thi**. Project này có ba minh hoạ rõ ràng: `bmad-build-story`, `lavish`, `run`.

### 4.2. `bmad-build-story`: workflow kiểm soát thay đổi

Skill này chia triển khai Story thành các bước chính:

1. **Clarify & Context**: lấy `story_id`, đọc sprint status, epic context, PRD/architecture và story trước đó; kiểm tra branch/worktree.
2. **Plan & Lavish visualization**: khảo sát code; lập spec Markdown; tạo HTML review; mở session/poll; dừng ở Checkpoint 1.
3. **Implement**: chỉ tiếp tục khi spec `ready-for-dev` và có phê duyệt; ghi baseline commit; code theo thứ tự phụ thuộc; build/test; tick tasks.
4. **Review**: lấy diff từ baseline, xem bằng các lăng kính Blind Hunter, Edge Case Hunter, Verification Gap; triage phát hiện.
5. **Hoàn tất/handoff**: cập nhật trạng thái, lưu evidence và báo lại.

Điểm then chốt là biến Story thành một finite-state workflow:

```text
backlog / ready-for-dev
          │ nhận Story
          ▼
spec: draft ──review Lavish──► ready-for-dev
          │                         │
          │ chưa được duyệt          ▼
          └─────────────── HALT  in-progress (có baseline commit)
                                           │
                                           ▼
                                      in-review
                                           │
                            ┌──────────────┴─────────────┐
                            ▼                            ▼
                     bad_spec / intent_gap          patch được sửa
                            │                            │
                            └──── quay về rõ yêu cầu ─────┘
                                           │
                                           ▼
                                         done
```

#### Checkpoint 1 là cơ chế gì?

Checkpoint 1 không phải chỉ là “hãy hỏi người dùng”. Trong skill có câu cấm rõ: agent tuyệt đối không tự viết/sửa code trước phê duyệt. Trước checkpoint, agent được phép điều tra và tạo hai sản phẩm đồng bộ:

- `_bmad-output/implementation-artifacts/spec-{story_key}.md`
- `.lavish/plan-story-{story_key}.html`

Ý đồ, phạm vi và ma trận edge case được đóng trong thẻ `<frozen-after-approval>`. Sau phê duyệt, agent chuyển spec sang `ready-for-dev`. Nhờ vậy, nếu code đi sai, đội dự án có thể đối chiếu nó với ý định con người đã duyệt thay vì tranh luận về nội dung chat cũ.

#### Format spec là “hợp đồng triển khai”

Spec mẫu trong project có các thành phần đáng kế thừa:

- frontmatter: title, type, created, status, `baseline_commit`, `review_loop_iteration`, context;
- **Intent**: Problem và Approach ngắn gọn;
- **Boundaries & Constraints**: Always/Never;
- **I/O & Edge-Case Matrix**: scenario, input/state, expected behavior, error handling;
- **Code Map**: `[NEW]`/`[MODIFY]` và mục đích file;
- **Tasks & Acceptance**: task theo file, lý do và Given/When/Then;
- Spec Change Log, Review Triage Log, Design Notes, Verification.

Spec 2.2 trong repo là bằng chứng rằng format này được dùng thực tế: nêu quyết định nghiệp vụ, các điều cấm, ma trận API/permission/concurrency, file thay đổi từng tầng và test cần có. Đây là loại tài liệu agent có thể triển khai trực tiếp mà người review vẫn đọc được.

#### Review đa lăng kính

Skill phân loại phát hiện thành `intent_gap`, `bad_spec`, `patch`, và (theo ý nghĩa workflow) tình huống cần dừng. Tách loại lỗi mang lại lợi ích lớn:

- `intent_gap`: không được tự đoán; cần người sở hữu nghiệp vụ quyết định.
- `bad_spec`: phải sửa “hợp đồng” trước rồi sửa code để tránh tái phát.
- `patch`: lỗi cục bộ, agent có thể sửa và verify.

Đây là một kỹ thuật tốt hơn việc trả về danh sách bug phẳng vì nó làm rõ ai có thẩm quyền xử lý và có cần quay lại checkpoint hay không.

### 4.3. `lavish`: kênh review chứ không phải agent

Skill Lavish hướng dẫn tạo/mở HTML để người dùng review. `AGENTS.md` bổ sung quy trình 5 phần riêng của project:

1. HTML nằm trong `.lavish/`.
2. Dùng `npx -y lavish-axi <artifact.html>` để mở session và gửi link session cho người dùng.
3. Dùng `npx -y lavish-axi poll <artifact.html>` chạy nền để chờ feedback.
4. Khi poll xong, agent phải đọc đầy đủ file log, không suy diễn từ thông báo tóm tắt.
5. Sau khi xử lý feedback, poll tiếp với `--agent-reply`; lặp cho tới khi người dùng Send & End.

Lavish không “đánh thức” model hay cấp quyền cho AI. Nó chỉ là kênh truyền feedback có ngữ cảnh. Agent phải đang tồn tại, có thể đọc log và được framework gọi tiếp thì vòng lặp mới tiếp diễn.

Template `lavish-plan-template.html` cho thấy artifact mong đợi gồm: toggle theme, tóm tắt, sơ đồ data flow, Code Map, I/O matrix, checklist, acceptance criteria và hộp Checkpoint 1. Nghĩa là HTML là **view của spec**, không phải một mockup vô thưởng vô phạt.

### 4.4. `run`: nắm bắt tri thức vận hành local

Skill run giải quyết một lỗi agent/developer hay gặp: chạy hai worktree lại trùng cổng và vô tình chạm vào ứng dụng của nhau. Quy tắc của project:

- `main`/`master`: API HTTP/HTTPS 5080/7080, Angular 4200, không sửa config.
- Feature/worktree: tính cổng 5081–5095 từ hash branch hoặc dùng cổng cho trước; HTTPS = HTTP + 2000; FE có dải 4201–4215.
- Đồng bộ hai đầu bằng cách sửa `BE/src/API/Properties/launchSettings.json` và `FE/proxy.conf.json`.
- Build backend trước, sau đó mới khởi động API/FE nền.

`run.ps1` hiện thực hóa phần này. Nó là ví dụ tốt về nguyên tắc **instruction + automation**: người viết không chỉ yêu cầu agent “hãy dùng cổng riêng” mà còn cung cấp script thực hiện phần có tính cơ học.

Lưu ý khi sao chép: script này sửa file cấu hình trong worktree. Điều đó hợp lý với worktree riêng nhưng có thể tạo diff không mong muốn nếu chạy trên branch chung. Project mới nên chuyển cổng sang `.env.local`, launch profile sinh tạm, hoặc ít nhất yêu cầu script báo rõ file sẽ đổi trước khi ghi.

---

## 5. BMAD: lớp quản lý tri thức và tiến độ

### 5.1. Chuỗi artifact planning → implementation

Project đang duy trì đường dẫn truy xuất như sau:

```text
Nghiệp vụ và quyết định sản phẩm
  └─ _bmad-output/planning-artifacts/
       ├─ prd.md
       ├─ epics.md
       ├─ architecture/.../ARCHITECTURE-SPINE.md
       └─ ux-designs/.../DESIGN.md, EXPERIENCE.md

Ngữ cảnh thực thi và bằng chứng
  └─ _bmad-output/implementation-artifacts/
       ├─ sprint-status.yaml
       ├─ epic-{n}-context.md
       ├─ spec-{story-key}.md
       ├─ bmad_review_diff_*.diff
       └─ bmad_review_claims_*.txt
```

Không phải mọi repository cần cả bộ artifact này. Nhưng với hệ thống có nghiệp vụ dày, nhiều Epic, nhiều vai trò và yêu cầu audit, nó có tác dụng như external memory cho người và agent.

### 5.2. `sprint-status.yaml` là state store, không chỉ là dashboard

Tệp theo dõi Epic và Story bằng trạng thái như `backlog`, `ready-for-dev`, `in-progress`, `review`, `done`. Skill build đọc nó để chọn story tiếp theo; agent cập nhật nó khi vượt qua checkpoint và khi hoàn thành. Vì thế, file này là giao diện hợp đồng giữa các phiên agent khác nhau.

Điều cần học: chỉ để agent “nhớ trong chat” rằng một Story đang review là không đủ. State phải được ghi vào repo, có diff Git, có thể kiểm tra sau vài ngày hoặc bởi agent khác.

### 5.3. Context theo Epic chống mất ngữ cảnh

`epic-1-context.md`, `epic-2-context.md` giúp agent chỉ cần nạp kiến thức kề cận khi triển khai. Đây là phân mảnh ngữ cảnh hợp lý:

- PRD/architecture toàn cục dùng để xác định luật lớn.
- Epic context gom quyết định của một khu vực nghiệp vụ.
- Spec Story mô tả thay đổi ngay trước mắt.
- Code và test là sự thật hiện hành cần kiểm chứng.

Từ đó có thể rút ra một chiến lược context budget: **đừng luôn nhét toàn bộ tài liệu vào prompt**. Hãy để skill chỉ rõ thứ tự đọc và chỉ mở sâu hơn khi Story cần.

### 5.4. Render snapshot và tính tái lập

`_bmad/scripts/render_skill.py` làm nhiều việc đáng chú ý:

- nạp central config và customization;
- thay token cấu hình vào workflow;
- tạo generation directory có hash dựa trên source, cấu hình, đường dẫn project và renderer;
- viết manifest kèm SHA-256 output;
- nếu directory đã tồn tại, kiểm tra toàn bộ manifest/hashes thay vì ghi đè.

Đây là một cơ chế chống “workflow drift”: một lần triển khai biết chính xác phiên bản hướng dẫn nào đã được render. Nếu project bạn cần audit cao, đây là ý tưởng rất tốt. Với project nhỏ, có thể đơn giản hóa thành commit hash của skill/template vào frontmatter spec.

---

## 6. Flow làm việc được suy ra từ project

### 6.1. Flow đầy đủ cho một Story

| Giai đoạn | Agent phải làm | Artifact/state | Ai kiểm soát |
|---|---|---|---|
| 0. Tiếp nhận | Nhận story/bug; xác định scope | Story key | Người dùng/PM |
| 1. Nạp context | Đọc sprint, Epic, PRD, architecture, spec trước đó, code hiện tại | Ghi chú điều tra | Agent, có thể hỏi lại |
| 2. Lập spec | Chuyển yêu cầu thành Intent, Never/Always, I/O matrix, Code Map, AC, Verification | `spec-*.md: draft` | Agent soạn, người dùng sẽ duyệt |
| 3. Trực quan hóa | Tạo plan HTML, khởi tạo Lavish/poll | `.lavish/plan-*.html`, log feedback | Người dùng annotate |
| 4. Checkpoint 1 | Dừng; sửa spec theo feedback; chỉ khi được duyệt mới mở khoá | `ready-for-dev`, frozen block | Người dùng |
| 5. Baseline | Ghi commit hiện tại và state work item | `baseline_commit`, `in-progress` | Agent |
| 6. Code | Sửa theo task/AC và kiến trúc; không nới scope ngầm | Diff source/test | Agent |
| 7. Verify | Build FE/BE, unit/E2E phù hợp | Log/kết quả test | Tool/CI |
| 8. Review | So diff baseline, blind/edge/verification lenses, triage | Review log/diff | Agent + người dùng nếu intent/spec đổi |
| 9. Handoff | Cập nhật spec/status, báo file, test và rủi ro còn lại | `done`/`review` | Agent, người review xác nhận |

### 6.2. Flow ngắn cho câu hỏi hoặc thay đổi nhỏ

Không nên bắt mọi việc đi qua toàn bộ 9 giai đoạn. Nếu một yêu cầu là giải thích, tra cứu, hoặc sửa chính tả không tác động hành vi, workflow phải nhẹ hơn:

```text
Yêu cầu nhỏ → đọc file liên quan → sửa tối thiểu → kiểm tra phù hợp → báo kết quả
```

Nguyên tắc phân loại đề xuất:

- **Tier 0 – Read-only**: trả lời, phân tích, báo cáo; không tạo spec.
- **Tier 1 – Safe micro-change**: thay đổi cục bộ, dễ đảo ngược; có thể không cần Lavish nhưng vẫn phải test/lint tương ứng.
- **Tier 2 – Feature/bug liên quan nhiều tầng**: dùng spec, code map, test plan, checkpoint review.
- **Tier 3 – Thay đổi kiến trúc/dữ liệu/bảo mật**: ngoài Tier 2, bắt buộc ADR hoặc migration plan, backup/rollback, review owner.

Nếu không phân cấp, skill nặng sẽ làm agent chậm và người dùng ngại dùng; nếu bỏ checkpoint cho mọi việc lớn, agent dễ vượt ý định.

---

## 7. Những điểm mạnh thực sự của thiết kế này

1. **Ràng buộc có khả năng hành động.** “Không dùng Data Annotation” đi kèm đúng nơi phải dùng Fluent API. “Không mock FE” đi kèm đúng nơi seed/test nên đặt dữ liệu.
2. **Tách ý định con người khỏi chi tiết triển khai.** Frozen block bảo vệ Problem/Approach/constraint sau phê duyệt; Code Map và task vẫn có thể là vùng kỹ thuật quản lý có kiểm soát.
3. **Có state xuyên phiên.** Sprint status, epic context, spec giúp agent mới nối tiếp được agent cũ.
4. **Review trước code cho thay đổi lớn.** Checkpoint 1 giảm nguy cơ tạo hàng loạt code đúng kỹ thuật nhưng sai nghiệp vụ.
5. **Có bằng chứng kiểm tra.** Baseline commit, diff, test, review triage giúp người review kiểm chứng claim “đã xong”.
6. **Tự động hoá phần lặp.** Script runner giảm lỗi cấu hình worktree/cổng.
7. **Cấu hình bền qua cập nhật.** Tách installer-managed, custom team và custom user là mô hình configuration hygiene tốt.
8. **Tối ưu cho domain cụ thể.** Thuật ngữ Việt, Velzon, Clean Architecture và công nghệ được ghi rõ thay vì agent phải đoán.

---

## 8. Giới hạn và rủi ro cần hiểu trước khi sao chép

### 8.1. Instruction không phải security boundary

Một câu trong Markdown không thể tự ngăn agent ghi file, push secret hay gọi lệnh phá huỷ. Khi cần chặn thật, phải dùng quyền công cụ, protected branch, secret manager, policy CI và approval trên hạ tầng.

Ví dụ cho project mới:

- Không commit `.env`; kiểm bằng secret scanning.
- Không cho token deploy trong agent environment mặc định.
- Bắt buộc PR approval để merge protected branch.
- Migration production chạy trong release job riêng, có backup và approval.

### 8.2. Checkpoint chỉ mạnh khi nền tảng tôn trọng nó

Checkpoint 1 trong skill rất rõ, nhưng hiệu lực phụ thuộc agent đọc skill và platform không có instruction cấp cao hơn mâu thuẫn. Để tăng hiệu lực, có thể thêm kiểm tra máy:

- script `verify-spec-ready` từ chối chạy implement nếu frontmatter chưa `ready-for-dev`;
- CI kiểm `baseline_commit`, acceptance criteria và test log cho Story lớn;
- CODEOWNERS yêu cầu owner duyệt những vùng kiến trúc/dữ liệu.

### 8.3. Lavish đòi runtime và vòng lặp sống

Lavish không phải magic. Nó cần Node.js đủ phiên bản, quyền chạy `npx`, browser/local session và agent process đang poll. Nếu session hoặc daemon bị dừng, note vẫn có thể được lưu nhưng agent sẽ không tự xử lý cho tới khi có một phiên agent đọc log.

### 8.4. Quá nhiều chữ tạo “compliance theater”

Nếu spec 1.600 token cho mọi thay đổi 2 dòng, agent và người dùng sẽ copy template cho đủ mục. Hãy đo chất lượng bằng:

- Có câu hỏi nghiệp vụ nào được giải quyết trước code không?
- Acceptance criteria có test được không?
- Code Map có khớp diff cuối cùng không?
- Review có bắt được lỗi thật không?
- Lead time có còn hợp lý không?

### 8.5. Một số quy tắc hiện có cần kiểm chứng tự động

Nhiều rule hiện tốt về mặt văn bản nhưng chưa thấy cơ chế bắt buộc tương ứng trong phạm vi khảo sát. Ví dụ: cấm data annotation, cấm mock data hoặc cấm hard-code màu. Với project mới, hãy viết analyzer/lint/grep test chính xác thay vì tin rằng agent luôn nhớ.

### 8.6. Chú ý file generated và đường dẫn máy cá nhân

Project đã cảnh báo `_bmad/config.toml` và `config.user.toml` là installer-managed. Template của bạn phải chỉ team sửa `_bmad/custom/config.toml`, không sửa generated config. Ngoài ra cần thay các URL `file:///c:/...` gắn máy cá nhân bằng relative path; nếu không team khác và CI sẽ không truy được.

---

## 9. Blueprint áp dụng cho project của bạn

### 9.1. Kiến trúc thư mục tối thiểu, đủ dùng

```text
your-project/
├─ AGENTS.md
├─ .agents/
│  └─ skills/
│     ├─ build-feature/SKILL.md
│     ├─ run/SKILL.md
│     └─ review-plan/SKILL.md              # chỉ thêm nếu có công cụ review thực sự
├─ docs/
│  ├─ product/PRD.md
│  ├─ architecture/ARCHITECTURE.md
│  ├─ adr/
│  └─ agent-playbook.md
├─ artifacts/
│  ├─ planning/
│  └─ implementation/
│     ├─ sprint-status.yaml
│     ├─ epic-1-context.md
│     └─ spec-<story>.md
├─ .lavish/                               # optional; gitignore nếu chỉ local
├─ scripts/
│  ├─ verify-spec-ready.*
│  └─ verify-agent-policy.*
└─ .github/workflows/ci.yml                # hoặc CI tương đương
```

Không cần cài toàn bộ BMAD để áp dụng tư duy này. Ba thành phần quan trọng nhất cần có từ ngày đầu là `AGENTS.md`, một spec feature có state, và lệnh verify chuẩn.

### 9.2. Mẫu `AGENTS.md` gọn, có thể dùng làm khởi điểm

```md
# Hướng dẫn cho Agent

## Mục tiêu
- [Mô tả 2–4 dòng về sản phẩm và người dùng.]

## Cây kiến trúc
- `apps/web/`: ...
- `apps/api/`: ...
- `packages/domain/`: ...

## Quy tắc bất biến
- Domain không phụ thuộc framework/ORM.
- Mọi thay đổi schema phải có migration và rollback note.
- Không đưa secret, dữ liệu khách hàng hoặc token vào source/log/test fixture.
- UI dùng design tokens; không tự thêm mock data vào production component.

## Thay đổi theo mức rủi ro
- Tier 0–1: đọc/sửa nhỏ, chạy kiểm tra liên quan.
- Tier 2: phải tạo `artifacts/implementation/spec-<id>.md` và nhận phê duyệt trước khi code.
- Tier 3: phải có ADR/migration plan/rollback và owner phê duyệt.

## Kiểm chứng tối thiểu
- Backend: `[lệnh test]`
- Frontend: `[lệnh lint/build/test]`
- E2E: `[lệnh]` khi thay đổi luồng người dùng.

## Cách báo cáo
- Nêu file thay đổi, hành vi, lệnh đã chạy và kết quả.
- Nêu rõ phần chưa kiểm tra hoặc rủi ro còn lại.
```

Đừng sao chép các rule NCKH về Velzon, .NET hoặc tiếng Việt nếu project bạn không dùng chúng. Một `AGENTS.md` mạnh là file có tính **địa phương**, không phải bộ luật phổ quát.

### 9.3. Mẫu skill xây dựng feature

```md
---
name: build-feature
description: Triển khai feature có ảnh hưởng nhiều module hoặc thay đổi hành vi người dùng.
---

1. Đọc `AGENTS.md`, PRD/ADR liên quan, trạng thái sprint và code hiện tại.
2. Tạo `artifacts/implementation/spec-<id>.md` với:
   - Intent, In scope/Out of scope
   - Invariants và security/privacy constraints
   - I/O + edge cases
   - Code map, task, acceptance criteria, verification
3. Dừng và xin phê duyệt nếu thay đổi thuộc Tier 2 hoặc Tier 3.
4. Sau phê duyệt, ghi baseline commit; triển khai theo task.
5. Chạy các lệnh verification; không báo hoàn thành nếu chúng thất bại.
6. Review diff so với baseline; phân loại intent_gap / bad_spec / patch.
7. Cập nhật spec/state và handoff bằng chứng.
```

Điểm quan trọng: prompt cần nói **điều kiện chuyển trạng thái**, không chỉ liệt kê hoạt động. “Hãy viết spec rồi code” yếu hơn “chỉ code khi `status: ready-for-dev` và approval được ghi nhận”.

### 9.4. Frontmatter và frozen intent đề xuất

```yaml
---
id: PAY-142
title: Hoàn tiền giao dịch thất bại
type: feature
status: draft                 # draft | ready-for-dev | in-progress | in-review | done
created: 2026-09-08
baseline_commit: null
approved_by: null
approved_at: null
context:
  - docs/product/PRD.md#hoan-tien
  - docs/architecture/ARCHITECTURE.md#payments
verification:
  - pnpm test payments
  - pnpm lint
---

<frozen-after-approval>
## Intent
...
## Invariants
...
## I/O and edge cases
...
</frozen-after-approval>
```

Sau approval, chỉ người có thẩm quyền mới được thay đổi phần frozen. Nếu yêu cầu nghiệp vụ đổi, tạo change log và tái approval; không lặng lẽ sửa intent để “hợp thức hoá” code đã viết.

### 9.5. Biến rule thành kiểm tra tự động

| Rule văn bản | Cơ chế nên thêm | Ví dụ |
|---|---|---|
| Không lộ secret | Secret scan pre-commit + CI | Gitleaks, GitHub secret scanning, trufflehog. |
| Không import sai tầng | Architecture test | ArchUnit/NetArchTest/dependency-cruiser/ESLint boundaries. |
| Không data annotation ở Domain | Unit test/reflection hoặc static scan | Fail nếu namespace Domain tham chiếu annotation cấm. |
| Không hard-code theme color | Stylelint/custom lint | Cấm literal color ngoài token file được whitelist. |
| Không mock data production | ESLint/custom rule | Cấm fixture import/mảng literal tại production pages theo convention. |
| Không code trước approval | Script + CI | CI fail nếu story Tier 2 có source diff mà spec thiếu `approved_at`/baseline. |
| Claim “đã test” có bằng chứng | CI required checks | Không để agent tự xác nhận thay CI. |

Automation không thay thế review nghiệp vụ nhưng chuyển các sai lầm lặp lại từ “nhớ giúp” thành “không thể qua cổng”.

### 9.6. Vận hành với agent khác hoặc môi trường khác

Mô hình này có thể dùng với Codex, Claude Code, Cursor, Gemini CLI hoặc agent nội bộ nếu môi trường đó hỗ trợ đọc repo và chạy tool. Phần cần thay là adapter, không phải triết lý:

| Khả năng | Nếu nền tảng có | Nếu nền tảng không có |
|---|---|---|
| Auto-discover `AGENTS.md` | Đặt ở root và dùng bình thường | Đưa nội dung vào project prompt/agent config, vẫn commit file làm nguồn chuẩn. |
| Skills | Đặt `SKILL.md` theo convention của nền tảng | Chuyển thành commands/playbooks hoặc prompt slash command. |
| Background poll | Dùng Lavish loop | Dùng PR/comment/ticket/form rồi gọi agent lại bằng workflow/webhook. |
| Terminal | Chạy build/test/script | Để CI chạy; agent chỉ soạn patch/plan và người dùng chạy local. |
| Multi-agent | Delegation có phân quyền | Dùng review checklist tuần tự, tránh giả định agents chạy đồng thời. |

Đừng phụ thuộc vào tên thư mục `.agents` hay `_bmad`. Giá trị chuyển giao là: **instruction discovery + artifact states + approval gates + verification evidence**.

---

## 10. Lộ trình áp dụng 30 ngày

### Tuần 1 — Đặt nền tối thiểu

- Viết `AGENTS.md` một trang, chỉ ghi kiến trúc, kiểm chứng, secrets, boundaries và workflow tier.
- Chuẩn hoá lệnh local/CI: bootstrap, lint, unit test, build, E2E.
- Tạo `.gitignore` cho `.env`, artifacts local, logs; thiết lập secret scan.
- Chọn 3–5 rule agent thường vi phạm nhất và thêm test/lint nếu có thể.

**Dấu hiệu thành công:** một agent mới có thể trả lời “sửa ở đâu, chạy test gì, không được làm gì” mà không hỏi nhiều vòng.

### Tuần 2 — Thử spec-first trên 1–2 feature rủi ro vừa

- Tạo template spec với Intent, boundaries, edge cases, code map, AC, verification.
- Bắt checkpoint phê duyệt cho migration, auth, payment, data deletion hoặc feature nhiều module.
- Lưu `baseline_commit` và review diff sau triển khai.

**Dấu hiệu thành công:** review phát hiện ít nhất một hiểu lầm trước khi code, hoặc có thể chứng minh không có hiểu lầm.

### Tuần 3 — Thêm external memory và review

- Thêm sprint-status đơn giản: YAML/Issue tracker là đủ.
- Viết epic context ngắn khi một nhóm feature có nhiều quyết định dùng chung.
- Dùng taxonomy `intent_gap`, `bad_spec`, `patch` trong review.
- Nếu người dùng thích review trực quan, thêm Lavish; nếu không, Markdown + PR comments là đủ.

**Dấu hiệu thành công:** agent thứ hai có thể tiếp tục feature mà không cần đọc toàn bộ lịch sử chat.

### Tuần 4 — Đo, cắt bớt, củng cố

- Đo lead time, số lần approval, finding review, failure CI và số rule bị vi phạm.
- Xoá bước không tạo giá trị; rút template nếu thành boilerplate.
- Chuyển các finding lặp lại thành test/lint hoặc pitfall trong `AGENTS.md`.
- Tách config generated/team/user nếu dùng tool có installer.

**Dấu hiệu thành công:** workflow giúp giảm rework mà không biến mỗi sửa đổi nhỏ thành thủ tục nặng nề.

---

## 11. Checklist đánh giá trước khi sao chép

### Phần bắt buộc

- [ ] `AGENTS.md` mô tả đúng codebase hiện tại, không sao chép rule không liên quan.
- [ ] Có lệnh build/test thật sự chạy được, không phải placeholder.
- [ ] Tài liệu sản phẩm/kiến trúc có owner và được cập nhật khi quyết định đổi.
- [ ] Story lớn có intent, boundaries, AC và verification rõ.
- [ ] Có cơ chế ghi state qua phiên agent: file hoặc issue tracker.
- [ ] Thay đổi rủi ro có human approval trước implementation/deploy.
- [ ] Không tin instruction như security boundary; secrets/production có quyền và CI bảo vệ.

### Phần chỉ thêm khi cần

- [ ] Persona nhiều vai trò khi team thật sự hưởng lợi từ các lens khác nhau.
- [ ] Lavish khi người review cần hình/luồng/table để ra quyết định tốt hơn Markdown.
- [ ] Render snapshot/hash khi cần audit/tái lập workflow cao.
- [ ] Worktree runner khi nhiều người/agent chạy song song local.
- [ ] Multi-agent runtime khi có cách cô lập quyền, chi phí và hợp nhất kết quả.

---

## 12. Kết luận

Điều chủ project này làm tốt là biến agent từ một chatbot trả lời theo trí nhớ thành một **tác nhân làm việc trong hệ thống có quy trình**. “Tri thức” không nhốt trong một prompt; nó được phân phối hợp lý:

- `AGENTS.md` giữ hiến pháp;
- skill giữ cách làm theo tác vụ;
- PRD/architecture/epic giữ quyết định;
- spec và sprint status giữ ngữ cảnh/nguyên trạng;
- Lavish hoặc review giữ quyền kiểm soát của con người;
- build/test/CI giữ bằng chứng kỹ thuật.

Khi áp dụng cho project của bạn, hãy bắt đầu tối giản: một `AGENTS.md` tốt, một template spec có approval, một bộ lệnh kiểm chứng đáng tin và một state file/issue tracker. Chỉ thêm BMAD, Lavish, persona hoặc multi-agent khi chúng giải quyết một vấn đề quan sát được. Mục tiêu không phải “làm agent trông thông minh hơn”, mà là làm thay đổi của agent **có chủ đích, truy vết được, kiểm chứng được và an toàn hơn**.

## Phụ lục A — Nguồn đã khảo sát trong repository

- `AGENTS.md`
- `.agents/skills/bmad-build-story/SKILL.md`
- `.agents/skills/bmad-build-story/templates/lavish-plan-template.html`
- `.agents/skills/lavish/SKILL.md`
- `.agents/skills/run/SKILL.md`
- `.agents/skills/run/scripts/run.ps1`
- `_bmad/config.toml`, `_bmad/config.user.toml`, `_bmad/custom/config.toml`
- `_bmad/bmm/config.yaml`, `_bmad/core/config.yaml`, `_bmad/_config/manifest.yaml`, `_bmad/_config/skill-manifest.csv`
- `_bmad/scripts/resolve_config.py`, `_bmad/scripts/resolve_customization.py`, `_bmad/scripts/render_skill.py`
- `_bmad-output/planning-artifacts/` và `_bmad-output/implementation-artifacts/`, gồm `sprint-status.yaml`, Epic context và các spec mẫu.
- `skills-lock.json`

## Phụ lục B — Câu hỏi tự đánh giá cho owner của project mới

1. Agent hay làm sai điều gì lặp lại? Câu trả lời phải biến thành test/lint hoặc rule ngắn.
2. Quyết định nào chỉ con người có quyền chốt? Đặt checkpoint ngay trước hành động không đảo ngược.
3. Artifact nào cần sống lâu hơn cuộc hội thoại? Commit nó vào repo hoặc issue tracker.
4. Câu “đã xong” cần bằng chứng gì? Xác định lệnh test, screenshot, migration report hoặc link CI.
5. Khi agent khác tiếp quản, nó phải đọc tối thiểu file nào? Viết thứ tự đó trong skill.
6. Workflow đang giảm rework hay chỉ tăng formality? Đo và loại bỏ bước không tạo giá trị.
