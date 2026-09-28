import docx, copy, os, shutil, sys
from docx.shared import Inches, Pt, Cm, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.enum.table import WD_TABLE_ALIGNMENT
from docx.oxml import OxmlElement, parse_xml
from docx.oxml.ns import nsdecls, qn

sys.path.append('scratch')
import data_attribute_tables as dat

sys.stdout.reconfigure(encoding='utf-8')
print("Starting v3 builder with full attribute tables, 100% black font, and first-line indents...")

src_path = 'files/KhoaLuanTotNghiep.docx'
dst_path = 'files/BaoCao_TienDo_GiuaKy_PTTKHTTT.docx'
backup_path = 'files/BaoCao_TienDo_GiuaKy_PTTKHTTT.backup_v3.docx'

try:
    shutil.copyfile(dst_path, backup_path)
    print(f"Created safety backup: {backup_path}")
except Exception as e:
    print(f"Backup notice: {e}")

thesis_doc = docx.Document(src_path)
current_doc = docx.Document(dst_path)

# Extract tables from thesis_doc
table_60_uc_el = copy.deepcopy(thesis_doc.tables[8]._element)
spec_tables = [copy.deepcopy(thesis_doc.tables[i]._element) for i in range(9, 18)]

# Extract screening table from current_doc
table_screening_el = None
for t in current_doc.tables:
    txt = t.cell(0, 0).text.strip()
    if 'Candidate' in txt or 'Lớp dự kiến' in txt:
        table_screening_el = copy.deepcopy(t._element)
        break

body = current_doc._body._element

p3_start_idx = None
p5_start_idx = None

for i, el in enumerate(body):
    tag = el.tag.split('}')[-1]
    if tag == 'p':
        t_nodes = el.xpath('.//w:t')
        txt = ''.join([t.text for t in t_nodes if t.text]).strip()
        if txt.startswith('PHẦN 3:'):
            p3_start_idx = i
        elif txt.startswith('PHẦN 5:'):
            p5_start_idx = i

print(f"Boundary: p3_start_idx={p3_start_idx}, p5_start_idx={p5_start_idx}")
p5_node = body[p5_start_idx]

# Remove legacy elements between Section 3 and Section 5
elements_to_remove = [body[i] for i in range(p3_start_idx, p5_start_idx)]
for el in elements_to_remove:
    body.remove(el)

print(f"Removed {len(elements_to_remove)} legacy elements.")

# Styling Constants
COLOR_BLACK = RGBColor(0, 0, 0)
COLOR_WHITE = RGBColor(255, 255, 255)
COLOR_NAVY_HEX = "0F2C59"

def make_heading_1(text):
    p = current_doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.LEFT
    p.paragraph_format.space_before = Pt(18)
    p.paragraph_format.space_after = Pt(6)
    p.paragraph_format.line_spacing = 1.5
    p.paragraph_format.first_line_indent = Inches(0)
    p.paragraph_format.keep_with_next = True
    run = p.add_run(text)
    run.font.name = 'Times New Roman'
    run.font.size = Pt(14)
    run.font.bold = True
    run.font.color.rgb = COLOR_BLACK
    p5_node.addprevious(p._element)
    return p

def make_heading_2(text):
    p = current_doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.LEFT
    p.paragraph_format.space_before = Pt(12)
    p.paragraph_format.space_after = Pt(6)
    p.paragraph_format.line_spacing = 1.5
    p.paragraph_format.first_line_indent = Inches(0)
    p.paragraph_format.keep_with_next = True
    run = p.add_run(text)
    run.font.name = 'Times New Roman'
    run.font.size = Pt(13)
    run.font.bold = True
    run.font.color.rgb = COLOR_BLACK
    p5_node.addprevious(p._element)
    return p

def make_heading_3(text):
    p = current_doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.LEFT
    p.paragraph_format.space_before = Pt(6)
    p.paragraph_format.space_after = Pt(6)
    p.paragraph_format.line_spacing = 1.5
    p.paragraph_format.first_line_indent = Inches(0)
    p.paragraph_format.keep_with_next = True
    run = p.add_run(text)
    run.font.name = 'Times New Roman'
    run.font.size = Pt(13)
    run.font.bold = True
    run.font.italic = True
    run.font.color.rgb = COLOR_BLACK
    p5_node.addprevious(p._element)
    return p

def make_body_p(text):
    p = current_doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
    p.paragraph_format.line_spacing = 1.5
    p.paragraph_format.first_line_indent = Cm(1.5)
    p.paragraph_format.space_before = Pt(6)
    p.paragraph_format.space_after = Pt(6)
    run = p.add_run(text)
    run.font.name = 'Times New Roman'
    run.font.size = Pt(13)
    run.font.color.rgb = COLOR_BLACK
    p5_node.addprevious(p._element)
    return p

def make_caption(text):
    p = current_doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p.paragraph_format.space_before = Pt(6)
    p.paragraph_format.space_after = Pt(6)
    p.paragraph_format.line_spacing = 1.2
    p.paragraph_format.first_line_indent = Inches(0)
    p.paragraph_format.keep_with_next = True
    run = p.add_run(text)
    run.font.name = 'Times New Roman'
    run.font.size = Pt(11.5)
    run.font.bold = True
    run.font.italic = True
    run.font.color.rgb = COLOR_BLACK
    p5_node.addprevious(p._element)
    return p

def make_picture(image_path, width_inches):
    if not os.path.exists(image_path):
        alt = image_path.replace("files/", "files/images/")
        if os.path.exists(alt):
            image_path = alt
    p = current_doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p.paragraph_format.space_before = Pt(6)
    p.paragraph_format.space_after = Pt(2)
    p.paragraph_format.first_line_indent = Inches(0)
    run = p.add_run()
    run.add_picture(image_path, width=Inches(width_inches))
    p5_node.addprevious(p._element)
    return p

def insert_raw_table(table_element):
    p5_node.addprevious(table_element)

def add_attribute_table(headers, data, col_widths=None):
    tbl = current_doc.add_table(rows=len(data) + 1, cols=len(headers))
    tbl.alignment = WD_TABLE_ALIGNMENT.CENTER
    tbl.autofit = False
    
    # Format header
    hdr_cells = tbl.rows[0].cells
    for i, h in enumerate(headers):
        hdr_cells[i].text = h
        shading = parse_xml(f'<w:shd {nsdecls("w")} w:fill="{COLOR_NAVY_HEX}"/>')
        hdr_cells[i]._element.get_or_add_tcPr().append(shading)
        p = hdr_cells[i].paragraphs[0]
        p.alignment = WD_ALIGN_PARAGRAPH.CENTER
        p.paragraph_format.space_before = Pt(3)
        p.paragraph_format.space_after = Pt(3)
        p.paragraph_format.first_line_indent = Inches(0)
        for r in p.runs:
            r.font.name = 'Times New Roman'
            r.font.size = Pt(13)
            r.font.bold = True
            r.font.color.rgb = COLOR_WHITE
            
    # Format data rows
    for row_idx, row_data in enumerate(data):
        row_cells = tbl.rows[row_idx + 1].cells
        bg_color = "F8FAFC" if row_idx % 2 == 1 else "FFFFFF"
        for col_idx, val in enumerate(row_data):
            row_cells[col_idx].text = str(val)
            shd = parse_xml(f'<w:shd {nsdecls("w")} w:fill="{bg_color}"/>')
            row_cells[col_idx]._element.get_or_add_tcPr().append(shd)
            p = row_cells[col_idx].paragraphs[0]
            p.paragraph_format.space_before = Pt(2)
            p.paragraph_format.space_after = Pt(2)
            p.paragraph_format.line_spacing = 1.15
            p.paragraph_format.first_line_indent = Inches(0)
            if col_idx in (0, 3):
                p.alignment = WD_ALIGN_PARAGRAPH.CENTER
            else:
                p.alignment = WD_ALIGN_PARAGRAPH.LEFT
            for r in p.runs:
                r.font.name = 'Times New Roman'
                r.font.size = Pt(13)
                r.font.color.rgb = COLOR_BLACK
                if col_idx == 1:
                    r.font.bold = False
                
    if col_widths:
        for r in tbl.rows:
            for i, w in enumerate(col_widths):
                r.cells[i].width = Inches(w)
                
    p5_node.addprevious(tbl._element)
    return tbl

# Format Section 1 and Section 2 paragraphs according to DNTU standards
print("Formatting Section 1 and Section 2 paragraphs according to DNTU standards...")
for p in current_doc.paragraphs:
    txt = p.text.strip()
    if not txt:
        continue
    if 'TRƯỜNG ĐẠI HỌC' in txt or 'BÁO CÁO TIẾN ĐỘ' in txt or 'Giảng viên hướng dẫn' in txt or '—————' in txt:
        continue
    if txt.startswith('Bảng 1:') or txt.startswith('Ghi chú:'):
        continue
    if txt.startswith('PHẦN 1') or txt.startswith('PHẦN 2'):
        p.paragraph_format.space_before = Pt(18)
        p.paragraph_format.space_after = Pt(6)
        p.paragraph_format.line_spacing = 1.5
        p.paragraph_format.first_line_indent = Inches(0)
        p.paragraph_format.keep_with_next = True
        for r in p.runs:
            r.font.name = 'Times New Roman'
            r.font.size = Pt(14)
            r.font.bold = True
            r.font.color.rgb = COLOR_BLACK
    elif any(txt.startswith(f'{c}.') and len(txt) > 2 and txt[2].isdigit() for c in (1, 2)):
        p.paragraph_format.space_before = Pt(12)
        p.paragraph_format.space_after = Pt(6)
        p.paragraph_format.line_spacing = 1.5
        p.paragraph_format.first_line_indent = Inches(0)
        p.paragraph_format.keep_with_next = True
        for r in p.runs:
            r.font.name = 'Times New Roman'
            r.font.size = Pt(13)
            r.font.bold = True
            r.font.color.rgb = COLOR_BLACK
    elif txt.startswith('Bảng '):
        p.paragraph_format.space_before = Pt(6)
        p.paragraph_format.space_after = Pt(6)
        p.paragraph_format.line_spacing = 1.2
        p.paragraph_format.first_line_indent = Inches(0)
        p.paragraph_format.keep_with_next = True
        p.alignment = WD_ALIGN_PARAGRAPH.CENTER
        for r in p.runs:
            r.font.name = 'Times New Roman'
            r.font.size = Pt(11.5)
            r.font.bold = True
            r.font.italic = True
            r.font.color.rgb = COLOR_BLACK
    elif txt.startswith('• ') or txt.startswith('- '):
        p.paragraph_format.space_before = Pt(3)
        p.paragraph_format.space_after = Pt(3)
        p.paragraph_format.line_spacing = 1.5
        p.paragraph_format.left_indent = Cm(1.5)
        p.paragraph_format.first_line_indent = Cm(-0.6)
        p.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
        for r in p.runs:
            r.font.name = 'Times New Roman'
            r.font.size = Pt(13)
            r.font.color.rgb = COLOR_BLACK
    else:
        # Body text of Section 1 and Section 2
        p.paragraph_format.line_spacing = 1.5
        p.paragraph_format.space_before = Pt(6)
        p.paragraph_format.space_after = Pt(6)
        p.paragraph_format.first_line_indent = Cm(1.5)
        p.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
        for r in p.runs:
            r.font.name = 'Times New Roman'
            r.font.size = Pt(13)
            r.font.color.rgb = COLOR_BLACK

print("Generating Section 3 content...")

# =========================================================================
# PHẦN 3: TIẾN ĐỘ PHÂN TÍCH USE CASE VÀ ĐẶC TẢ CHI TIẾT
# =========================================================================
make_heading_1("PHẦN 3: TIẾN ĐỘ PHÂN TÍCH USE CASE VÀ ĐẶC TẢ CHI TIẾT")

make_heading_2("3.1. Hệ thống Tác nhân và Mô hình Use Case tổng thể")
make_body_p("Trong quy trình phân tích hướng đối tượng (OOAD), việc xác định ranh giới hệ thống (System Boundary) và các tác nhân tương tác là bước nền tảng để bảo đảm hệ thống vận hành đúng mục tiêu và độc lập về mặt dữ liệu. Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (PFM) thiết lập ranh giới bảo mật 100% hướng tới người dùng cá nhân, loại bỏ hoàn toàn các quyền can thiệp quản trị viên (Admin) vào số dư tiền bạc nhằm bảo đảm quyền riêng tư tài chính tuyệt đối.")
make_body_p("Hệ thống xác lập rõ ràng cấu trúc gồm một Tác nhân chính duy nhất và ba Tác nhân phụ trợ ngoại vi:")
make_body_p("Tác nhân chính (Primary Actor) của hệ thống là Người dùng cá nhân (Personal User / Saver). Đây là tác nhân con người duy nhất trực tiếp tương tác với ứng dụng di động để quản lý tài khoản, đa ví, ghi nhận thu chi, thiết lập ngân sách chu kỳ, theo dõi lộ trình trả nợ và tiếp nhận các cú hích hỗ trợ ra quyết định tài chính.")
make_body_p("Tác nhân phụ trợ AI Vision OCR Engine là hệ thống trí tuệ nhân tạo thị giác máy tính đa phương thức, tiếp nhận ảnh chụp hóa đơn mua lẻ, nhận diện ký tự quang học, phân rã danh sách các mặt hàng chi tiết và trả về cấu trúc JSON phục vụ bóc tách tự động.")
make_body_p("Tác nhân phụ trợ Cổng VietQR Gateway là dịch vụ thanh toán chuyển khoản liên ngân hàng Napas 247 theo chuẩn EMVCo Merchant-Presented Mode, tự động mã hóa thông tin tài khoản đích, số tiền và nội dung thanh toán thành mã QR động.")
make_body_p("Tác nhân phụ trợ Dịch vụ Thông báo Push Notification Service là hạ tầng điều phối tin nhắn vi mô theo thời gian thực, truyền tải các cảnh báo vượt hạn mức chi tiêu mỗi ngày (DSA), cảnh báo vận tốc tiêu tiền nguy hiểm và thông báo thời gian trễ tâm lý hạ nhiệt (Cooling-off delay) đến thiết bị của người dùng.")

make_picture("files/ucd_00_overview.png", 5.8)
make_caption("Hình 3.1: Sơ đồ Use Case tổng thể hệ thống (UCD 00 - Tổng Quát)")

make_body_p("Sơ đồ Use Case tổng thể hệ thống (Hình 3.1) biểu diễn trực quan mối liên kết giữa Primary Actor với 9 khối gói chức năng (Packages) đại diện cho toàn bộ quy trình quản lý dòng tiền khép kín. Các phân hệ tương tác xuyên suốt thông qua các sự kiện miền, bảo đảm tính nhất quán dữ liệu và trải nghiệm người dùng liền mạch.")

make_heading_2("3.2. Danh mục Phân loại 60 Use Cases theo 9 Phân hệ Nghiệp vụ")
make_body_p("Căn cứ trên mô hình I-P-O và các yêu cầu chức năng khảo sát hiện trạng, nhóm đã phân rã toàn bộ hệ thống thành 60 ca sử dụng con chi tiết thuộc 9 phân hệ chức năng cốt lõi. Bảng 4 dưới đây tổng hợp danh mục phân loại, định danh mã chuẩn và mức độ ưu tiên triển khai theo chuẩn mực học thuật quốc tế:")

make_caption("Bảng 4: Bảng tổng hợp phân loại 60 Use Cases theo 9 phân hệ chức năng")
insert_raw_table(table_60_uc_el)

make_heading_2("3.3. Sơ đồ Use Case và Thuyết minh Nghiệp vụ Chi tiết 9 Phân hệ (UCD 01 - UCD 09)")
make_body_p("Nhằm làm sáng tỏ hành vi và các quy tắc nghiệp vụ chuyên biệt của từng phân hệ, nhóm đã thiết kế 9 bản vẽ sơ đồ Use Case phân tầng tương ứng với 9 tabs nghiệp vụ trên công cụ Draw.io, đồng thời thuyết minh sâu sắc các cơ chế vận hành cốt lõi:")

# 3.3.1
make_heading_3("3.3.1. Phân hệ Quản lý Tài khoản & Đa Ví cá nhân (UCD 01)")
make_picture("files/ucd_01_wallet.png", 5.5)
make_caption("Hình 3.2: Sơ đồ Use Case Phân hệ Quản lý Tài khoản & Đa Ví cá nhân (UCD 01)")
make_body_p("Phân hệ Quản lý Tài khoản & Đa Ví cá nhân thiết lập nền tảng lưu trữ và quản trị toàn bộ các kênh lưu chuyển tiền tệ của người dùng. Hệ thống hỗ trợ 4 loại ví tài sản chuyên biệt: Ví tiền mặt vật lý (Cash Wallet), Tài khoản ngân hàng (Bank Account), Thẻ tín dụng với cơ chế chu kỳ sao kê và hạn mức tín dụng (Credit Card Wallet), và Ví điện tử (E-Wallet).")
make_body_p("Quy tắc nghiệp vụ trọng tâm của phân hệ là cơ chế chuyển tiền nội bộ giữa hai ví (Internal Transfer) được thực thi trong một giao dịch nguyên tử (ACID Transaction) sử dụng cơ chế khóa dòng bi quan (Pessimistic Row-level Locking). Điều này bảo đảm số tiền bị khấu trừ ở ví nguồn và số tiền cộng vào ví đích phải hoàn tất đồng thời 100%, tuyệt đối không xảy ra tình trạng thất thoát hoặc sai lệch số dư. Bên cạnh đó, tính năng cân chỉnh số dư (Balance Adjustment) ghi nhận các giao dịch chênh lệch thực tế kèm biên bản giải trình bất biến, giúp người dùng luôn duy trì sự trung thực với bức tranh tài chính cá nhân.")

# 3.3.2
make_heading_3("3.3.2. Phân hệ Quản lý Thu nhập cá nhân (UCD 02)")
make_picture("files/ucd_02_income.png", 5.5)
make_caption("Hình 3.3: Sơ đồ Use Case Phân hệ Quản lý Thu nhập cá nhân (UCD 02)")
make_body_p("Phân hệ Quản lý Thu nhập phụ trách việc ghi nhận, phân loại và điều phối toàn bộ các nguồn tiền đi vào hệ thống tài chính cá nhân. Hệ thống phân định rành mạch giữa Thu nhập chủ động (tiền lương cố định từ công việc chính, thù lao theo giờ) và Thu nhập thụ động/biến đổi (lợi nhuận đầu tư, cổ tức, thu nhập freelance bán thời gian).")
make_body_p("Để giải quyết cạm bẫy tâm lý lạc quan túng thiếu cuối tháng (khi có một khoản thu bất thường khiến người dùng chi tiêu xả láng), hệ thống tích hợp cơ chế Vùng đệm an toàn thu nhập (Income Buffer). Cơ chế này tự động đề xuất trích lập một tỷ lệ cố định của các khoản thu đột xuất vào Quỹ dự phòng khẩn cấp trước khi phân bổ số tiền còn lại vào ngân sách chi tiêu hàng ngày.")

# 3.3.3
make_heading_3("3.3.3. Phân hệ Chi tiêu & Bóc tách Hóa đơn AI OCR (UCD 03)")
make_picture("files/ucd_03_expense_ocr.png", 5.8)
make_caption("Hình 3.4: Sơ đồ Use Case Phân hệ Chi tiêu & Bóc tách Hóa đơn AI OCR (UCD 03)")
make_body_p("Phân hệ Chi tiêu giải quyết triệt để rào cản lớn nhất của các phần mềm tài chính truyền thống: sự phiền toái và tốn thời gian khi phải nhập liệu thủ công từng khoản chi tiêu nhỏ lẻ. Hệ thống tích hợp Pipeline AI Vision OCR đa phương thức (Multimodal AI), cho phép người dùng chỉ cần chụp một bức ảnh hóa đơn bán lẻ siêu thị hoặc nhà hàng, mô hình AI sẽ tự động phân tích và trích xuất từng dòng sản phẩm (Itemized Line-items) bao gồm tên mặt hàng, số lượng, đơn giá, thuế VAT và chiết khấu.")
make_body_p("Điểm đột phá về mặt nghiệp vụ là Thuật toán Kiểm toán số học 3 tầng (Mathematical Audit Engine) được tích hợp bắt buộc (<<include>>). Thuật toán tự động đối soát công thức: Tổng tiền = Tổng(Đơn giá x Số lượng) + Thuế VAT - Giảm giá. Nếu phát hiện sai lệch số học vượt ngưỡng dung sai cho phép (do ảnh bị mờ hoặc rách), hệ thống đánh dấu trạng thái cảnh báo và chuyển sang chế độ Human-in-the-loop, yêu cầu người dùng xác nhận lại trước khi chính thức ghi nhận trừ tiền vào ví tài sản. Ngoài ra, phân hệ hỗ trợ tạo mã thanh toán VietQR động theo chuẩn EMVCo và quét nhận diện các bẫy đăng ký thuê bao định kỳ (Zombie Subscriptions).")

# 3.3.4
make_heading_3("3.3.4. Phân hệ Quản lý Danh mục Thu Chi & Quy tắc thông minh (UCD 04)")
make_picture("files/ucd_04_category.png", 5.8)
make_caption("Hình 3.5: Sơ đồ Use Case Phân hệ Quản lý Danh mục Thu Chi & Quy tắc thông minh (UCD 04)")
make_body_p("Phân hệ Quản lý Danh mục thiết lập cấu trúc phân loại hình cây hai cấp (Cha - Con) khoa học, phản ánh chính xác thói quen tiêu dùng cá nhân. Điểm độc đáo của hệ thống là khả năng ánh xạ đồng thời cây danh mục với hai trường phái tài chính kinh điển thế giới: Quy tắc 50/30/20 (Thiết yếu - Nhu cầu - Tích lũy) và Phương pháp Kakeibo Nhật Bản 4 cột trụ (Needs - Wants - Culture - Unexpected).")
make_body_p("Động cơ Quy tắc thông minh (Smart Categorization Rules Engine) cho phép người dùng thiết lập các biểu thức chính quy (Regex Pattern) để tự động nhận diện từ khóa trong nội dung chuyển khoản hoặc tên mặt hàng hóa đơn (ví dụ: từ khóa 'Grab', 'Be' tự động phân vào danh mục 'Di chuyển', từ khóa 'Highlands', 'Starbucks' tự động gán nhãn 'Cà phê & Ăn vặt'), giúp giảm tải 90% thao tác thủ công của người dùng.")

# 3.3.5
make_heading_3("3.3.5. Phân hệ Quản lý Ngân sách & Hạn mức chi tiêu DSA (UCD 05)")
make_picture("files/ucd_05_budget_dsa.png", 5.8)
make_caption("Hình 3.6: Sơ đồ Use Case Phân hệ Quản lý Ngân sách & Hạn mức chi tiêu DSA (UCD 05)")
make_body_p("Phân hệ Quản lý Ngân sách cung cấp công cụ định hướng và duy trì kỷ luật tài chính trong từng chu kỳ tháng hoặc tuần. Hệ thống hỗ trợ đa dạng phương pháp thiết lập: Ngân sách tỷ lệ 50/30/20, Ngân sách dựa trên số không (Zero-Based Budgeting - ZBB gán việc cho từng đồng tiền), và Ngân sách phong bì Kakeibo.")
make_body_p("Thay vì chỉ đưa ra một hạn mức cố định theo tháng gây cảm giác trừu tượng, phân hệ chuyển hóa ngân sách chu kỳ thành chỉ số Hạn mức chi tiêu mỗi ngày khả dụng thích ứng (Daily Spending Allowance - DSA). Chỉ số này liên tục được tính toán lại theo thời gian thực: DSA = (Ngân sách danh mục còn lại) / (Số ngày còn lại của chu kỳ). Khi người dùng chi tiêu tiết kiệm trong hôm nay, hạn mức của các ngày sau sẽ tự động tăng lên; ngược lại, nếu chi tiêu quá tay, DSA các ngày tới sẽ tự động siết lại kèm theo cảnh báo Tốc độ tiêu tiền (Spending Velocity) vượt ngưỡng an toàn.")

# 3.3.6
make_heading_3("3.3.6. Phân hệ Quỹ Tích lũy & Tối ưu hóa Nợ cá nhân (UCD 06)")
make_picture("files/ucd_06_savings_debt.png", 5.5)
make_caption("Hình 3.7: Sơ đồ Use Case Phân hệ Quỹ Tích lũy & Tối ưu hóa Nợ cá nhân (UCD 06)")
make_body_p("Phân hệ Quỹ Tích lũy & Tối ưu hóa Nợ hỗ trợ người dùng đạt được sự cân bằng hài hòa giữa việc xây dựng tài sản tương lai và giải phóng các nghĩa vụ tài chính trong quá khứ. Hệ thống hướng dẫn người dùng thiết lập mục tiêu Quỹ khẩn cấp (Emergency Fund) tương đương từ 3 đến 6 tháng sinh hoạt phí thiết yếu và tự động trích lập tiền tiết kiệm mỗi khi có dòng tiền thu nhập mới.")
make_body_p("Đối với các nghĩa vụ nợ cá nhân (thẻ tín dụng, vay tiêu dùng, nợ người thân), phân hệ tích hợp Bộ giải thuật toán tối ưu hóa nợ độc quyền, cung cấp bảng phân tích so sánh song song giữa hai chiến lược trả nợ chuẩn mực: Debt Snowball (ưu tiên tất toán các khoản nợ có số dư nhỏ nhất trước để tạo hưng phấn tâm lý) và Debt Avalanche (ưu tiên dồn tiền trả khoản nợ có lãi suất phần trăm cao nhất trước để tối ưu hóa tiết kiệm tiền lãi về mặt toán học). Người dùng có thể chạy mô phỏng và chọn chiến lược phù hợp nhất với điều kiện thực tế của bản thân.")

# 3.3.7
make_heading_3("3.3.7. Phân hệ Báo cáo & Phân tích Tài chính Đa chiều (UCD 07)")
make_picture("files/ucd_07_analytics.png", 5.5)
make_caption("Hình 3.8: Sơ đồ Use Case Phân hệ Báo cáo & Phân tích Tài chính Đa chiều (UCD 07)")
make_body_p("Phân hệ Báo cáo & Phân tích Tài chính đảm nhiệm vai trò tổng hợp và trực quan hóa toàn bộ dữ liệu dòng tiền. Báo cáo Dòng tiền thuần (Net Cashflow = Tổng Thu - Tổng Chi) và Đường xu hướng Giá trị Tài sản ròng (Net Worth Trend = Tổng Tài sản - Tổng Nợ) giúp người dùng đánh giá chính xác sức khỏe tài chính tổng thể qua từng mốc thời gian.")
make_body_p("Các biểu đồ nhiệt (Heatmap) theo dõi thói quen chi tiêu theo khung giờ trong tuần và phân tích cơ cấu chi phí theo nguyên lý Pareto 80/20 giúp người dùng phát hiện chính xác 20% các nhóm chi tiêu tùy chọn nhưng đang chiếm tới 80% ngân sách rò rỉ, từ đó có căn cứ dữ liệu để cắt giảm thông minh.")

# 3.3.8
make_heading_3("3.3.8. Phân hệ Dự báo Tài chính & Rủi ro Thâm hụt (UCD 08)")
make_picture("files/ucd_08_forecasting.png", 5.8)
make_caption("Hình 3.9: Sơ đồ Use Case Phân hệ Dự báo Tài chính & Rủi ro Thâm hụt (UCD 08)")
make_body_p("Phân hệ Dự báo Tài chính đóng vai trò như một hệ thống cảnh báo sớm (Early Warning System). Bằng cách kết hợp phân tích chuỗi thời gian của các chu kỳ chi tiêu lịch sử và vận tốc tiêu tiền hiện tại, hệ thống tính toán và dự phóng chỉ số Ngày cạn tiền khả dụng (Out-of-Money Date). Nếu ngày cạn tiền dự báo diễn ra trước ngày nhận lương tiếp theo, hệ thống sẽ kích hoạt tín hiệu báo động đỏ thâm hụt.")
make_body_p("Đặc biệt, phân hệ cung cấp Phòng thí nghiệm mô phỏng mua sắm giả định (What-If Sandbox). Người dùng có thể thử nghiệm một quyết định mua sắm tài sản lớn (ví dụ: mua một chiếc xe máy mới trị giá 30 triệu đồng) trong môi trường bộ nhớ ảo tạm thời. Hệ thống sẽ dự phóng tức thì đường cong dòng tiền trong 6 tháng tiếp theo nếu thực hiện khoản chi này, giúp người dùng tránh được các quyết định bốc đồng nguy hiểm.")

# 3.3.9
make_heading_3("3.3.9. Phân hệ Cú hích Hành vi & Hỗ trợ Quyết định Thaler (UCD 09)")
make_picture("files/ucd_09_nudge_thaler.png", 6.0)
make_caption("Hình 3.10: Sơ đồ Use Case Phân hệ Cú hích Hành vi & Hỗ trợ Quyết định Thaler (UCD 09)")
make_body_p("Phân hệ Cú hích Hành vi là linh hồn của đề tài, hiện thực hóa các nghiên cứu kinh tế học hành vi đoạt giải Nobel Kinh tế 2017 của Giáo sư Richard Thaler. Hệ thống phát sinh các Cú hích vi mô (Micro-Nudge) đúng thời điểm tại thời khắc người dùng chuẩn bị chi tiêu hoặc khi phát hiện một giao dịch làm vỡ hạn mức DSA.")
make_body_p("Mọi cú hích can thiệp đều đi kèm tính năng Trí tuệ nhân tạo có thể giải trình (Explainable AI - XAI), chỉ rõ căn cứ số liệu và đề xuất phương án tái cân đối động giữa các danh mục (ví dụ: chuyển 200.000đ từ danh mục Ăn ngoài sang danh mục Mua sắm). Quyền phê duyệt điều chỉnh dữ liệu số dư luôn thuộc về người dùng (Human-in-the-loop). Đồng thời, cơ chế Tạo khoảng trễ tâm lý (Cooling-off delay) áp dụng quy tắc tạm hoãn từ 24 đến 48 giờ đối với các khoản chi tùy ý giá trị lớn, khôi phục lại 'ma sát thanh toán' cần thiết để người dùng có thời gian hạ nhiệt cảm xúc và đưa ra quyết định lý trí.")

# 3.4
make_heading_2("3.4. Bộ Đặc tả Chi tiết Ca Sử dụng Cốt lõi Chuẩn Quốc tế IEEE 29148")
make_body_p("Đặc tả ca sử dụng là văn kiện kỹ thuật trung tâm trong quy trình phân tích hệ thống nhằm chuẩn hóa chi tiết từng bước tương tác giữa người dùng và phần mềm trong mọi kịch bản nghiệp vụ. Tuân thủ nghiêm ngặt chuẩn mực quốc tế IEEE 29148:2018 và phương pháp luận của Debra Paul & Alistair Cockburn, nhóm xây dựng các bảng đặc tả ca sử dụng theo khuôn dạng Bảng Dọc 2 x 14 gồm đầy đủ 13 trường thông tin tiêu chuẩn:")
make_body_p("Dưới đây là toàn văn 9 Bảng Đặc Tả Ca Sử Dụng Tổng Quát Cấp Hệ Thống (System-Level Macro Use Cases: UC-01 đến UC-09) đại diện cho 9 phân hệ chức năng cốt lõi được chuyển giao trực tiếp từ Khóa luận tốt nghiệp:")

spec_captions = [
    "Bảng 5: Đặc tả ca sử dụng UC-01: Quản lý tài khoản và Đa Ví cá nhân",
    "Bảng 6: Đặc tả ca sử dụng UC-02: Quản lý thu nhập cá nhân",
    "Bảng 7: Đặc tả ca sử dụng UC-03: Quản lý chi tiêu và Bóc tách hóa đơn thông minh",
    "Bảng 8: Đặc tả ca sử dụng UC-04: Quản lý danh mục thu chi và Quy tắc thông minh",
    "Bảng 9: Đặc tả ca sử dụng UC-05: Quản lý ngân sách đa phương pháp và Hạn mức chi tiêu khả dụng hàng ngày DSA",
    "Bảng 10: Đặc tả ca sử dụng UC-06: Quỹ tích lũy và Tối ưu hóa nghĩa vụ nợ cá nhân",
    "Bảng 11: Đặc tả ca sử dụng UC-07: Báo cáo và Phân tích tài chính đa chiều",
    "Bảng 12: Đặc tả ca sử dụng UC-08: Dự báo tài chính và Cảnh báo rủi ro thâm hụt",
    "Bảng 13: Đặc tả ca sử dụng UC-09: Cú hích hành vi và Hỗ trợ quyết định tài chính"
]

for idx, tbl_el in enumerate(spec_tables):
    make_caption(spec_captions[idx])
    insert_raw_table(tbl_el)

print("Generating Section 4 content with attribute tables...")

# =========================================================================
# PHẦN 4: TIẾN ĐỘ THIẾT KẾ CLASS DIAGRAM
# =========================================================================
make_heading_1("PHẦN 4: TIẾN ĐỘ THIẾT KẾ CLASS DIAGRAM")

make_heading_2("4.1. Quy trình 6 bước phân tích hướng đối tượng (OOAD) của Giảng viên")
make_body_p("Nhóm áp dụng nghiêm ngặt quy trình 6 bước phương pháp luận OOAD chuẩn của Giảng viên hướng dẫn để chuyển hóa các yêu cầu chức năng và đặc tả Use Case thành Sơ đồ Lớp nghiệp vụ hoàn chỉnh. Toàn bộ danh pháp các Lớp đối tượng (Class Names), Thuộc tính (Attributes) và Phương thức (Methods) đều được chuẩn hóa 100% bằng Tiếng Anh kỹ thuật theo quy chuẩn quốc tế UML và Domain-Driven Design (DDD):")
make_body_p("Bước 1: Tìm lớp dự kiến (Candidate Classes Identification). Trích xuất toàn bộ danh từ từ mô hình I-P-O và 60 ca sử dụng đã đặc tả để tập hợp các ứng viên ban đầu.")
make_body_p("Bước 2: Sàng lọc và loại bỏ lớp không phù hợp. Áp dụng triệt để 6 tiêu chí sàng lọc học thuật của Giảng viên để loại bỏ các lớp trùng lặp, đóng vai trò giao diện hay chức năng thuần túy.")
make_body_p("Bước 3: Xác định thuộc tính và chuẩn hóa kiểu dữ liệu CSDL. Chuẩn hóa thuộc tính, kiểu dữ liệu tương thích Microsoft SQL Server (MSSQL/T-SQL) và đóng gói Value Object Money nhằm triệt tiêu sai số số thực.")
make_body_p("Bước 4: Xác định phương thức miền nghiệp vụ giàu hành vi. Xây dựng Rich Domain Model, trang bị các phương thức tính toán tài chính phức tạp, xóa bỏ hoàn toàn mô hình Anemic Domain Model thụ động chỉ có getter và setter.")
make_body_p("Bước 5: Xác định các mối quan hệ giữa các lớp. Thiết lập chính xác các quan hệ Kế thừa (Generalization), Kết tập (Aggregation), Hợp thành (Composition), Liên kết (Association) và Lớp kết hợp (Associative Classes).")
make_body_p("Bước 6: Tổng hợp và đóng gói Sơ đồ lớp hoàn chỉnh. Mô hình hóa toàn diện trên công cụ Draw.io chuyên nghiệp với kiến trúc phân vùng Bounded Contexts rõ ràng.")

make_heading_2("4.2. Bảng đối chiếu sàng lọc loại bỏ lớp theo 6 tiêu chí học thuật")
make_body_p("Trong Bước 2, nhóm đã tiến hành sàng lọc khoa học đối với toàn bộ các ứng viên ban đầu để loại bỏ những lớp không thỏa mãn điều kiện trở thành thực thể miền nghiệp vụ (Domain Entities):")
make_caption("Bảng 14: Bảng đối chiếu sàng lọc và loại bỏ lớp theo 6 tiêu chí")
if table_screening_el is not None:
    insert_raw_table(table_screening_el)

make_heading_2("4.3. Giải pháp Kiến trúc Phân tầng 2 Cấp theo Domain-Driven Design (DDD)")
make_body_p("Sau quá trình sàng lọc học thuật nghiêm ngặt, hệ thống giữ lại chính xác 18 Lớp Thực thể Miền (Domain Entities) và 2 Đối tượng Giá trị Bất biến (Value Objects) với hàng trăm thuộc tính và phương thức tính toán. Một thách thức lớn đặt ra là khi đưa toàn bộ sơ đồ 20 lớp này vào một trang giấy khổ A4 duy nhất, kích thước chữ buộc phải co về 3pt - 4pt, khiến tài liệu trở nên mờ nhòe, không thể đọc được trên bản in và vi phạm tiêu chuẩn tài liệu kỹ thuật công nghiệp.")
make_body_p("Để giải quyết triệt để vấn đề này, nhóm tiếp cận theo mô hình Kiến trúc Phân tầng 2 Cấp (2-Level Architectural Hierarchy) chuẩn mực của Domain-Driven Design (DDD):")
make_body_p("Cấp độ 1 - Bản đồ Ngữ cảnh Tổng quan (Bounded Context Map - Tầm nhìn Vĩ mô): Cung cấp sơ đồ bao quát thể hiện ranh giới và mối quan hệ phụ thuộc giữa 6 Khối Ngữ cảnh lớn trong hệ thống mà không đi sâu vào chi tiết thuộc tính hay hàm, giúp Giảng viên và Hội đồng nắm bắt kiến trúc toàn cảnh trong 15 giây.")
make_body_p("Cấp độ 2 - Sơ đồ Lớp Chi tiết theo 6 Bounded Contexts (Tầm nhìn Vi mô): Phân rã sơ đồ lớn thành 6 sơ đồ lớp thành phần độc lập. Mỗi sơ đồ chỉ chứa từ 3 đến 5 lớp đối tượng liên quan mật thiết, cho phép hiển thị đầy đủ kiểu dữ liệu T-SQL và phương thức nghiệp vụ ở kích cỡ chữ 10pt - 12pt tiêu chuẩn A4 cực kỳ sắc nét.")

make_heading_2("4.4. Sơ đồ Kiến trúc Tổng quan và Bộ 6 Sơ đồ Lớp Phân hệ Chi tiết Kèm Bảng Thuộc Tính")
make_picture("files/cd_overview.png", 6.0)
make_caption("Hình 4.1: Sơ đồ kiến trúc tổng thể 6 Bounded Contexts miền PFM (Context Map)")
make_body_p("Sơ đồ kiến trúc tổng thể (Hình 4.1) mô tả sự phân bổ không gian và ranh giới nghiệp vụ của 6 Bounded Contexts miền tài chính cá nhân. Dưới đây là chi tiết từng sơ đồ phân hệ thành phần được trích xuất trực tiếp từ bản vẽ kỹ thuật Draw.io độ nét cao, đi kèm bảng đặc tả chi tiết thuộc tính của từng lớp đối tượng:")

# Context 1
make_heading_3("4.4.1. Context 1: Định danh & Hồ sơ cá nhân (Identity & Profile Context)")
make_picture("files/cd_context_1_identity.png", 5.5)
make_caption("Hình 4.2: Context 1: Định danh & Hồ sơ cá nhân (Identity & Profile Context)")
make_body_p("Ngữ cảnh 1 đóng vai trò hạt nhân trung tâm (Core Domain), định danh người dùng cá nhân (User) và quản trị các thuộc tính tài chính nền tảng như ngày nhận lương (salaryDay), mức độ chấp nhận rủi ro (riskTolerance) và tiền tệ mặc định. Ngữ cảnh này sở hữu 2 Đối tượng Giá trị bất biến cốt lõi dùng chung cho toàn bộ hệ thống: Money (đóng gói số tiền Decimal và mã tiền tệ) và DateRange (đóng gói chu kỳ ngày bắt đầu - ngày kết thúc).")

make_caption("Bảng 15: Bảng thuộc tính lớp thực thể User")
add_attribute_table(dat.ATTR_HEADERS, dat.USER_ATTRS, dat.COL_WIDTHS)

make_caption("Bảng 16: Bảng thuộc tính các Đối tượng giá trị Money và DateRange")
add_attribute_table(dat.ATTR_HEADERS, dat.VALUE_OBJECTS_ATTRS, dat.COL_WIDTHS)

# Context 2
make_heading_3("4.4.2. Context 2: Tài khoản & Đa ví cá nhân chuẩn ACID (Account & Multi-Wallet Context)")
make_picture("files/cd_context_2_wallet.png", 4.8)
make_caption("Hình 4.3: Context 2: Tài khoản & Đa ví cá nhân chuẩn ACID (Account & Multi-Wallet Context)")
make_body_p("Ngữ cảnh 2 quản lý toàn bộ các phương tiện thanh toán thông qua lớp cha trừu tượng Wallet và 4 lớp con kế thừa: CashWallet, BankAccountWallet, CreditCardWallet, và EWallet. Mọi hành vi nạp tiền (deposit), rút tiền (withdraw) và chuyển tiền nội bộ (InternalTransfer) đều được thực thi theo chuẩn ACID. Lớp BalanceAdjustment lưu vết lịch sử kiểm kê chênh lệch số dư thực tế.")

make_caption("Bảng 17: Bảng thuộc tính lớp trừu tượng Wallet và các lớp con kế thừa")
add_attribute_table(dat.ATTR_HEADERS, dat.WALLET_ATTRS, dat.COL_WIDTHS)

make_caption("Bảng 18: Bảng thuộc tính các lớp liên kết InternalTransfer và BalanceAdjustment")
add_attribute_table(dat.ATTR_HEADERS, dat.TRANSFER_ADJUSTMENT_ATTRS, dat.COL_WIDTHS)

# Context 3
make_heading_3("4.4.3. Context 3: Giao dịch, AI Vision OCR & Danh mục (Transactions, AI OCR & Categories Context)")
make_picture("files/cd_context_3_transactions.png", 4.8)
make_caption("Hình 4.4: Context 3: Giao dịch, AI Vision OCR & Danh mục (Transactions, AI OCR & Categories Context)")
make_body_p("Ngữ cảnh 3 quản lý dòng tiền biến động với lớp trừu tượng Transaction cùng 2 lớp con chuyên biệt IncomeTransaction và ExpenseTransaction. Điểm nhấn kiến trúc là mối quan hệ hợp thành (Composition) chặt chẽ giữa Hóa đơn bóc tách AI (Receipt) và các dòng mặt hàng (ReceiptItem), bảo đảm tính toàn vẹn của thuật toán kiểm toán số học verifyMathematicalAudit(). Cây danh mục Category hỗ trợ quan hệ Cha - Con đệ quy và liên kết với luật tự động hóa AutoCategorizationRule. Lớp Subscription giám sát các khoản trừ tiền lặp lại.")

make_caption("Bảng 19: Bảng thuộc tính lớp Transaction, IncomeTransaction và ExpenseTransaction")
add_attribute_table(dat.ATTR_HEADERS, dat.TRANSACTION_ATTRS, dat.COL_WIDTHS)

make_caption("Bảng 20: Bảng thuộc tính lớp Category và AutoCategorizationRule")
add_attribute_table(dat.ATTR_HEADERS, dat.CATEGORY_RULE_ATTRS, dat.COL_WIDTHS)

make_caption("Bảng 21: Bảng thuộc tính lớp Receipt và ReceiptItem")
add_attribute_table(dat.ATTR_HEADERS, dat.RECEIPT_ATTRS, dat.COL_WIDTHS)

make_caption("Bảng 22: Bảng thuộc tính lớp Subscription")
add_attribute_table(dat.ATTR_HEADERS, dat.SUBSCRIPTION_ATTRS, dat.COL_WIDTHS)

# Context 4
make_heading_3("4.4.4. Context 4: Ngân sách đa phương pháp & DSA (Budgeting & Adaptive DSA Context)")
make_picture("files/cd_context_4_budgeting.png", 5.8)
make_caption("Hình 4.5: Context 4: Ngân sách đa phương pháp & DSA (Budgeting & Adaptive DSA Context)")
make_body_p("Ngữ cảnh 4 hiện thực hóa các mô hình phân bổ ngân sách thông qua thực thể gốc Budget, lớp thành phần BudgetAllocation và thực thể tính toán DailySpendingAllowance (DSA). Thực thể DSA sở hữu các phương thức calculateDSA(), computeSpendingVelocity() và estimateOutOfMoneyDate(), liên tục điều chỉnh hạn mức ngày theo diễn biến chi tiêu thực tế.")

make_caption("Bảng 23: Bảng thuộc tính lớp Budget và BudgetAllocation")
add_attribute_table(dat.ATTR_HEADERS, dat.BUDGET_ATTRS, dat.COL_WIDTHS)

make_caption("Bảng 24: Bảng thuộc tính lớp DailySpendingAllowance (DSA)")
add_attribute_table(dat.ATTR_HEADERS, dat.DSA_ATTRS, dat.COL_WIDTHS)

# Context 5
make_heading_3("4.4.5. Context 5: Quỹ tích lũy & Tối ưu hóa Nợ cá nhân (Personal Debt & Financial Goals Context)")
make_picture("files/cd_context_5_debt_goals.png", 5.2)
make_caption("Hình 4.6: Context 5: Quỹ tích lũy & Tối ưu hóa Nợ cá nhân (Personal Debt & Financial Goals Context)")
make_body_p("Ngữ cảnh 5 tích hợp hai trụ cột tài chính dài hạn: Mục tiêu tích lũy (FinancialGoal cùng lớp con EmergencyFundGoal) và Nghĩa vụ nợ (DebtAccount). Kế hoạch trả nợ DebtPaymentPlan liên kết chặt chẽ với lịch trình thanh toán DebtRepaymentScheduleItem và giao dịch thanh toán nợ DebtPaymentTransaction, sở hữu các thuật toán mô phỏng simulateSnowball() và simulateAvalanche().")

make_caption("Bảng 25: Bảng thuộc tính lớp FinancialGoal, EmergencyFundGoal và GoalContribution")
add_attribute_table(dat.ATTR_HEADERS, dat.GOAL_ATTRS, dat.COL_WIDTHS)

make_caption("Bảng 26: Bảng thuộc tính lớp DebtAccount")
add_attribute_table(dat.ATTR_HEADERS, dat.DEBT_ACCOUNT_ATTRS, dat.COL_WIDTHS)

make_caption("Bảng 27: Bảng thuộc tính lớp DebtPaymentPlan, DebtRepaymentScheduleItem và DebtPaymentTransaction")
add_attribute_table(dat.ATTR_HEADERS, dat.DEBT_PLAN_TRANSACTION_ATTRS, dat.COL_WIDTHS)

# Context 6
make_heading_3("4.4.6. Context 6: Cú hích hành vi & Hỗ trợ quyết định Thaler (Decision Support & Behavioral Nudge Context)")
make_picture("files/cd_context_6_nudge.png", 5.5)
make_caption("Hình 4.7: Context 6: Cú hích hành vi & Hỗ trợ quyết định Thaler (Decision Support & Behavioral Nudge Context)")
make_body_p("Ngữ cảnh 6 điều phối các can thiệp tâm lý với thực thể FinancialNudge. Thực thể sở hữu các thuộc tính giải trình lý do minh bạch (rationaleExplanation), cờ xác nhận của người dùng (isApprovedByUser) và mốc thời gian tạm hoãn (coolingOffUntil) cùng các phương thức kích hoạt applyCoolingOff() và confirmHumanAction(), hiện thực hóa trọn vẹn triết lý kinh tế học hành vi Richard Thaler.")

make_caption("Bảng 28: Bảng thuộc tính lớp FinancialNudge")
add_attribute_table(dat.ATTR_HEADERS, dat.NUDGE_ATTRS, dat.COL_WIDTHS)

# 4.5
make_heading_2("4.5. Thiết kế Rich Domain Model & Value Object Bất Biến (Money)")
make_body_p("Áp dụng chặt chẽ phương pháp luận Domain-Driven Design (DDD), nhóm kiên quyết xóa bỏ mô hình Anemic Domain Model (các lớp chỉ chứa trường dữ liệu và getter/setter rỗng). Toàn bộ 18 lớp thực thể nghiệp vụ đều được trang bị các phương thức tính toán giàu hành vi (Rich Domain Methods) tương thích 100% với tên gọi trên Sơ đồ lớp Draw.io kỹ thuật:")
make_body_p("Thực thể Wallet (Ví tài chính): Sở hữu phương thức transferInternal() để thực thi chuyển tiền nội bộ nguyên tử, adjustBalance() để ghi nhận cân chỉnh số dư thực tế và calculateAvailableBalance() để tính số dư khả dụng.")
make_body_p("Thực thể Transaction (Giao dịch thu chi): Xử lý biến động dòng tiền, liên kết chặt chẽ với Category và Receipt để phân loại chi phí.")
make_body_p("Thực thể Budget (Kế hoạch ngân sách): Sở hữu phương thức checkOverspending() để kiểm tra vượt hạn mức theo chu kỳ và calculateRemaining() để tính số tiền khả dụng còn lại.")
make_body_p("Thực thể DebtAccount (Nghĩa vụ nợ cá nhân): Sở hữu phương thức calculateInterest() để tính lãi phát sinh định kỳ và simulateSnowball(), simulateAvalanche() để mô phỏng lộ trình thanh toán nợ tối ưu.")
make_body_p("Thực thể Receipt (Hóa đơn bóc tách AI): Sở hữu phương thức verifyMathematicalAudit() để kiểm toán công thức: Tổng tiền = Tổng(Đơn giá x Số lượng) + VAT - Giảm giá.")
make_body_p("Đặc biệt, hệ thống chuẩn hóa đối tượng giá trị bất biến Money (Value Object gồm amount: DECIMAL(18,2) và currency: NVARCHAR(10)). Việc đóng gói Money với các phương thức toán học an toàn (add, subtract, multiply) giúp loại trừ 100% lỗi sai số làm tròn số thực dấu phẩy động (Floating-point precision errors) trong các thuật toán chia nhỏ ngân sách, tính lãi suất nợ và cộng dồn hóa đơn đa mặt hàng.")

print("Updating captions in Section 5 and Q&A...")
for el in body:
    t_nodes = el.xpath('.//w:t')
    txt = ''.join([t.text for t in t_nodes if t.text]).strip()
    if 'Đánh giá tiến độ hoàn thành các hạng mục' in txt:
        for t in t_nodes:
            if 'Bảng' in t.text and ':' in t.text:
                t.text = 'Bảng 29: Đánh giá tiến độ hoàn thành các hạng mục công việc giữa kỳ'
    elif 'Bảng tra cứu các thuật ngữ viết tắt chuyên môn' in txt:
        for t in t_nodes:
            if 'Bảng' in t.text and ':' in t.text:
                t.text = 'Bảng 30: Bảng tra cứu các thuật ngữ viết tắt chuyên môn trong đề tài PFM'

# Format Section 5 and Q&A paragraphs
print("Formatting Section 5 and Q&A paragraphs according to DNTU standards...")
in_sec5 = False
for p in current_doc.paragraphs:
    txt = p.text.strip()
    if txt.startswith('PHẦN 5') or txt.startswith('PHẦN Q&A'):
        in_sec5 = True
    if in_sec5:
        if txt.startswith('PHẦN 5') or txt.startswith('PHẦN Q&A'):
            p.paragraph_format.space_before = Pt(18)
            p.paragraph_format.space_after = Pt(6)
            p.paragraph_format.line_spacing = 1.5
            p.paragraph_format.first_line_indent = Inches(0)
            p.paragraph_format.keep_with_next = True
            for r in p.runs:
                r.font.name = 'Times New Roman'
                r.font.size = Pt(14)
                r.font.bold = True
                r.font.color.rgb = COLOR_BLACK
        elif any(txt.startswith(f'{c}.') and len(txt) > 2 and txt[2].isdigit() for c in (5,)):
            p.paragraph_format.space_before = Pt(12)
            p.paragraph_format.space_after = Pt(6)
            p.paragraph_format.line_spacing = 1.5
            p.paragraph_format.first_line_indent = Inches(0)
            p.paragraph_format.keep_with_next = True
            for r in p.runs:
                r.font.name = 'Times New Roman'
                r.font.size = Pt(13)
                r.font.bold = True
                r.font.color.rgb = COLOR_BLACK
        elif txt.startswith('Bảng ') or txt.startswith('Hình '):
            p.paragraph_format.space_before = Pt(6)
            p.paragraph_format.space_after = Pt(6)
            p.paragraph_format.line_spacing = 1.2
            p.paragraph_format.first_line_indent = Inches(0)
            p.paragraph_format.keep_with_next = True
            p.alignment = WD_ALIGN_PARAGRAPH.CENTER
            for r in p.runs:
                r.font.name = 'Times New Roman'
                r.font.size = Pt(11.5)
                r.font.bold = True
                r.font.italic = True
                r.font.color.rgb = COLOR_BLACK
        elif txt.startswith('Câu hỏi ') or txt.startswith('Trả lời:'):
            p.paragraph_format.line_spacing = 1.5
            p.paragraph_format.space_before = Pt(6)
            p.paragraph_format.space_after = Pt(6)
            p.paragraph_format.first_line_indent = Cm(1.5)
            p.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
            for r in p.runs:
                r.font.name = 'Times New Roman'
                r.font.size = Pt(13)
                r.font.color.rgb = COLOR_BLACK
        elif len(txt) > 20:
            p.paragraph_format.line_spacing = 1.5
            p.paragraph_format.space_before = Pt(6)
            p.paragraph_format.space_after = Pt(6)
            p.paragraph_format.first_line_indent = Cm(1.5)
            p.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
            for r in p.runs:
                r.font.name = 'Times New Roman'
                r.font.size = Pt(13)
                r.font.color.rgb = COLOR_BLACK

print("Running global font color cleanup (100% black text, white only for navy table headers)...")
for p in current_doc.paragraphs:
    for r in p.runs:
        if r.font.color and r.font.color.rgb != COLOR_BLACK:
            r.font.color.rgb = COLOR_BLACK

for tbl in current_doc.tables:
    for row_idx, row in enumerate(tbl.rows):
        is_hdr = (row_idx == 0)
        for cell in row.cells:
            tcPr = cell._element.tcPr
            shd = tcPr.find(qn('w:shd')) if tcPr is not None else None
            fill = shd.get(qn('w:fill')) if shd is not None else None
            is_navy = (fill and fill.upper() in ("0F2C59", "1E3A8A"))
            
            for cp in cell.paragraphs:
                for cr in cp.runs:
                    cr.font.name = 'Times New Roman'
                    cr.font.size = Pt(13)
                    if is_navy:
                        cr.font.color.rgb = COLOR_WHITE
                        cr.font.bold = True
                    else:
                        cr.font.color.rgb = COLOR_BLACK
                        if row_idx > 0:
                            cr.font.bold = False

# Save first to working copy
out_temp = 'files/BaoCao_TienDo_GiuaKy_PTTKHTTT_v3.docx'
current_doc.save(out_temp)
print(f"Saved compiled version to: {out_temp}")

# Try to overwrite main file if not locked
try:
    shutil.copyfile(out_temp, dst_path)
    print(f"Successfully updated main document: {dst_path}")
except PermissionError:
    print(f"NOTE: {dst_path} is currently locked by Word. Created {out_temp} successfully!")

# Final Verification
v_doc = docx.Document(out_temp)
print("=== FINAL VERIFICATION ===")
print(f"Total paragraphs: {len(v_doc.paragraphs)}")
print(f"Total tables: {len(v_doc.tables)}")
img_count = len([k for k in v_doc.part.rels.values() if 'image' in k.target_ref])
print(f"Total embedded images: {img_count}")
file_size = os.path.getsize(out_temp)
print(f"File size: {file_size} bytes ({file_size / (1024*1024):.2f} MB)")
