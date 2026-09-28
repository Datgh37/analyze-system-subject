import docx
from docx import Document
from docx.shared import Inches, Pt, RGBColor, Cm
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.enum.table import WD_TABLE_ALIGNMENT
from docx.oxml import OxmlElement, parse_xml
from docx.oxml.ns import nsdecls, qn
import shutil
import os

print("=== BẮT ĐẦU QUÁ TRÌNH CHUẨN HÓA VÀ HỢP NHẤT KHÓA LUẬN TỐT NGHIỆP (DYNAMIC MATCHING) ===")

src_file = 'files/KhoaLuanTotNghiep.docx'
backup_file = 'files/KhoaLuanTotNghiep.pre_merge.docx'
shutil.copy2(src_file, backup_file)
print(f"1. Đã sao lưu an toàn: {backup_file}")

doc = Document(src_file)

# Helper: thiết lập font Times New Roman
def set_run_font(run, size_pt=13, bold=False, italic=False, color_rgb=(0, 0, 0)):
    run.font.name = 'Times New Roman'
    run.font.size = Pt(size_pt)
    run.bold = bold
    run.italic = italic
    run.font.color.rgb = RGBColor(*color_rgb)
    rPr = run._r.get_or_add_rPr()
    rFonts = rPr.find(qn('w:rFonts'))
    if rFonts is None:
        rFonts = parse_xml(f'<w:rFonts {nsdecls("w")} w:ascii="Times New Roman" w:hAnsi="Times New Roman" w:cs="Times New Roman"/>')
        rPr.append(rFonts)
    else:
        rFonts.set(qn('w:ascii'), 'Times New Roman')
        rFonts.set(qn('w:hAnsi'), 'Times New Roman')
        rFonts.set(qn('w:cs'), 'Times New Roman')

# Helper: định dạng đoạn văn chuẩn học thuật
def format_academic_paragraph(p, first_line_cm=1.5, left_cm=0, before_pt=6, after_pt=6, line_spacing=1.5, align=WD_ALIGN_PARAGRAPH.JUSTIFY):
    p.paragraph_format.first_line_indent = Cm(first_line_cm)
    p.paragraph_format.left_indent = Cm(left_cm)
    p.paragraph_format.space_before = Pt(before_pt)
    p.paragraph_format.space_after = Pt(after_pt)
    p.paragraph_format.line_spacing = line_spacing
    p.alignment = align

# Helper: định dạng đoạn bullet chuẩn 1/2 tab
def format_bullet_paragraph(p, before_pt=6, after_pt=6, line_spacing=1.5):
    p.paragraph_format.left_indent = Cm(0.75)
    p.paragraph_format.first_line_indent = Cm(-0.35)
    p.paragraph_format.space_before = Pt(before_pt)
    p.paragraph_format.space_after = Pt(after_pt)
    p.paragraph_format.line_spacing = line_spacing
    p.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY

# -------------------------------------------------------------
# BƯỚC 1: THỐNG NHẤT TÊN ĐỀ TÀI TRÊN TOÀN BỘ TÀI LIỆU
# -------------------------------------------------------------
print("2. Thống nhất Tên Đề Tài...")

for i, p in enumerate(doc.paragraphs[:60]):
    txt = p.text.strip()
    if "HỆ THỐNG QUẢN LÝ CHI TIÊU &" in txt:
        p.text = "HỆ THỐNG QUẢN LÝ CHI TIÊU VÀ HOẠCH ĐỊNH"
        p.alignment = WD_ALIGN_PARAGRAPH.CENTER
        for r in p.runs:
            set_run_font(r, size_pt=16, bold=True)
        print(f"  -> Cập nhật tên đề tài bìa dòng 1 (P{i})")
    elif "LẬP NGÂN SÁCH CÁ NHÂN" in txt and i < 15:
        p.text = "TÀI CHÍNH CÁ NHÂN THÔNG MINH (TÍCH HỢP TRÍ TUỆ NHÂN TẠO - AI)"
        p.alignment = WD_ALIGN_PARAGRAPH.CENTER
        for r in p.runs:
            set_run_font(r, size_pt=16, bold=True)
        print(f"  -> Cập nhật tên đề tài bìa dòng 2 (P{i})")
    elif "SMART EXPENSE & BUDGET MANAGER" in txt and i < 15:
        p.text = "(SMART PERSONAL EXPENSE AND FINANCIAL PLANNING MANAGEMENT SYSTEM WITH AI INTEGRATION)"
        p.alignment = WD_ALIGN_PARAGRAPH.CENTER
        for r in p.runs:
            set_run_font(r, size_pt=13, bold=True)
        print(f"  -> Cập nhật tên đề tài bìa tiếng Anh (P{i})")
    elif "1. Tên đề tài:" in txt:
        p.text = ""
        r1 = p.add_run("1. Tên đề tài: ")
        set_run_font(r1, size_pt=13, bold=True)
        r2 = p.add_run("HỆ THỐNG QUẢN LÝ CHI TIÊU VÀ HOẠCH ĐỊNH TÀI CHÍNH CÁ NHÂN THÔNG MINH (TÍCH HỢP TRÍ TUỆ NHÂN TẠO - AI)")
        set_run_font(r2, size_pt=13, bold=True)
        print(f"  -> Cập nhật BM205 Tên đề tài (P{i})")
    elif txt.startswith("Tên đề tài:") and "SMART EXPENSE" in txt:
        p.text = ""
        r1 = p.add_run("Tên đề tài: ")
        set_run_font(r1, size_pt=13, bold=True)
        r2 = p.add_run("HỆ THỐNG QUẢN LÝ CHI TIÊU VÀ HOẠCH ĐỊNH TÀI CHÍNH CÁ NHÂN THÔNG MINH (TÍCH HỢP TRÍ TUỆ NHÂN TẠO - AI)")
        set_run_font(r2, size_pt=13, bold=True)
        print(f"  -> Cập nhật BM208 Tên đề tài (P{i})")

# Đoạn dẫn đầu Chương 3
for p in doc.paragraphs:
    if "Hệ thống Quản lý Chi tiêu và Lập Ngân sách Cá nhân Thông minh" in p.text:
        p.text = p.text.replace(
            "Hệ thống Quản lý Chi tiêu và Lập Ngân sách Cá nhân Thông minh",
            "Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (Tích hợp Trí tuệ Nhân tạo - AI)"
        )
        for r in p.runs:
            set_run_font(r, size_pt=13)
        format_academic_paragraph(p)
        print("  -> Cập nhật đoạn mở đầu Chương 3")
        break

# -------------------------------------------------------------
# BƯỚC 2: CHÈN BẢNG PHÂN CÔNG NHIỆM VỤ VÀ TỶ LỆ ĐÓNG GÓP
# -------------------------------------------------------------
print("3. Chèn Bảng Phân Công Nhiệm Vụ vào cuối Khối thủ tục đầu...")

body = doc._body._body
tbl3_idx = None
for idx, el in enumerate(body):
    tag = el.tag.split('}')[-1]
    if tag == 'tbl' and idx > 60 and idx < 95:
        tbl3_idx = idx
        break

new_tbl = doc.add_table(rows=3, cols=6)
new_tbl.alignment = WD_TABLE_ALIGNMENT.CENTER

col_widths = [Inches(0.45), Inches(1.20), Inches(0.85), Inches(0.85), Inches(0.65), Inches(2.10)]
headers = ['STT', 'Họ và tên\nsinh viên', 'Mã SV', 'Vai trò', 'Đóng góp', 'Tóm tắt công việc đã thực hiện']
row1_data = [
    '01',
    'Trịnh Thành Đạt',
    '1923050149',
    'Trưởng nhóm',
    '70%',
    '- Chủ trì phân tích bài toán PFM cá nhân 100% và kiến trúc tổng thể.\n- Phân tích thị trường, đối chiếu ứng dụng hiện có và định vị khoảng trống nghiên cứu.\n- Xây dựng danh mục yêu cầu chức năng (9 phân hệ) và phi chức năng.\n- Mô hình hóa Use Case, phân định <<include>> / <<extend>>, vẽ sơ đồ Draw.io.\n- Biên tập và chịu trách nhiệm toàn bộ nội dung tài liệu.'
]
row2_data = [
    '02',
    'Trần Trọng Đại',
    '1923050745',
    'Thành viên',
    '30%',
    '- Hỗ trợ thu thập thông tin tính năng các ứng dụng thị trường (Money Manager, Spendee).\n- Khảo sát đặc điểm hành vi của 3 nhóm chân dung người dùng (Sinh viên, Người mới đi làm).\n- Hỗ trợ rà soát danh sách tên gọi các Use Case con và thuộc tính dữ liệu thô.\n- Hỗ trợ kiểm tra định dạng bảng biểu và rà soát lỗi chính tả văn bản.'
]

def set_cell_shading(cell, color_hex):
    tcPr = cell._tc.get_or_add_tcPr()
    shd = parse_xml(f'<w:shd {nsdecls("w")} w:fill="{color_hex}"/>')
    tcPr.append(shd)

def set_cell_margins(cell, top=100, bottom=100, left=140, right=140):
    tcPr = cell._tc.get_or_add_tcPr()
    tcMar = parse_xml(f'''
        <w:tcMar {nsdecls("w")}>
            <w:top w:w="{top}" w:type="dxa"/>
            <w:bottom w:w="{bottom}" w:type="dxa"/>
            <w:left w:w="{left}" w:type="dxa"/>
            <w:right w:w="{right}" w:type="dxa"/>
        </w:tcMar>
    ''')
    tcPr.append(tcMar)

def set_cell_borders(cell, border_hex="D3D3D3"):
    tcPr = cell._tc.get_or_add_tcPr()
    tcBorders = parse_xml(f'''
        <w:tcBorders {nsdecls("w")}>
            <w:top w:val="single" w:sz="4" w:space="0" w:color="{border_hex}"/>
            <w:left w:val="single" w:sz="4" w:space="0" w:color="{border_hex}"/>
            <w:bottom w:val="single" w:sz="4" w:space="0" w:color="{border_hex}"/>
            <w:right w:val="single" w:sz="4" w:space="0" w:color="{border_hex}"/>
        </w:tcBorders>
    ''')
    tcPr.append(tcBorders)

# Header Row
hdr_row = new_tbl.rows[0]
hdr_row._tr.get_or_add_trPr().append(parse_xml(f'<w:tblHeader {nsdecls("w")}/>'))
hdr_row._tr.get_or_add_trPr().append(parse_xml(f'<w:cantSplit {nsdecls("w")}/>'))
for col_idx, text in enumerate(headers):
    cell = hdr_row.cells[col_idx]
    cell.width = col_widths[col_idx]
    set_cell_shading(cell, "1F4E78")
    set_cell_margins(cell)
    set_cell_borders(cell, "FFFFFF")
    cell.paragraphs[0].text = ""
    cell.paragraphs[0].alignment = WD_ALIGN_PARAGRAPH.CENTER
    cell.paragraphs[0].paragraph_format.space_before = Pt(4)
    cell.paragraphs[0].paragraph_format.space_after = Pt(4)
    cell.paragraphs[0].paragraph_format.line_spacing = 1.15
    for line in text.split('\n'):
        if cell.paragraphs[0].text:
            p = cell.add_paragraph()
            p.alignment = WD_ALIGN_PARAGRAPH.CENTER
            p.paragraph_format.space_before = Pt(0)
            p.paragraph_format.space_after = Pt(4)
            p.paragraph_format.line_spacing = 1.15
            r = p.add_run(line)
        else:
            r = cell.paragraphs[0].add_run(line)
        set_run_font(r, size_pt=13, bold=True, color_rgb=(255, 255, 255))

# Row 1
r1 = new_tbl.rows[1]
r1._tr.get_or_add_trPr().append(parse_xml(f'<w:cantSplit {nsdecls("w")}/>'))
for col_idx, val in enumerate(row1_data):
    cell = r1.cells[col_idx]
    cell.width = col_widths[col_idx]
    set_cell_shading(cell, "FFFFFF")
    set_cell_margins(cell)
    set_cell_borders(cell, "D3D3D3")
    cell.paragraphs[0].text = ""
    cell.paragraphs[0].paragraph_format.space_before = Pt(3)
    cell.paragraphs[0].paragraph_format.space_after = Pt(3)
    cell.paragraphs[0].paragraph_format.line_spacing = 1.25
    if col_idx in [0, 2, 3, 4]:
        cell.paragraphs[0].alignment = WD_ALIGN_PARAGRAPH.CENTER
    else:
        cell.paragraphs[0].alignment = WD_ALIGN_PARAGRAPH.LEFT
    lines = val.split('\n')
    for l_idx, line in enumerate(lines):
        if l_idx == 0:
            p = cell.paragraphs[0]
        else:
            p = cell.add_paragraph()
            p.paragraph_format.space_before = Pt(2)
            p.paragraph_format.space_after = Pt(2)
            p.paragraph_format.line_spacing = 1.25
            if col_idx in [0, 2, 3, 4]:
                p.alignment = WD_ALIGN_PARAGRAPH.CENTER
            else:
                p.alignment = WD_ALIGN_PARAGRAPH.LEFT
        r = p.add_run(line)
        set_run_font(r, size_pt=13, bold=False, color_rgb=(0, 0, 0))

# Row 2 (Shaded)
r2 = new_tbl.rows[2]
r2._tr.get_or_add_trPr().append(parse_xml(f'<w:cantSplit {nsdecls("w")}/>'))
for col_idx, val in enumerate(row2_data):
    cell = r2.cells[col_idx]
    cell.width = col_widths[col_idx]
    set_cell_shading(cell, "F2F4F7")
    set_cell_margins(cell)
    set_cell_borders(cell, "D3D3D3")
    cell.paragraphs[0].text = ""
    cell.paragraphs[0].paragraph_format.space_before = Pt(3)
    cell.paragraphs[0].paragraph_format.space_after = Pt(3)
    cell.paragraphs[0].paragraph_format.line_spacing = 1.25
    if col_idx in [0, 2, 3, 4]:
        cell.paragraphs[0].alignment = WD_ALIGN_PARAGRAPH.CENTER
    else:
        cell.paragraphs[0].alignment = WD_ALIGN_PARAGRAPH.LEFT
    lines = val.split('\n')
    for l_idx, line in enumerate(lines):
        if l_idx == 0:
            p = cell.paragraphs[0]
        else:
            p = cell.add_paragraph()
            p.paragraph_format.space_before = Pt(2)
            p.paragraph_format.space_after = Pt(2)
            p.paragraph_format.line_spacing = 1.25
            if col_idx in [0, 2, 3, 4]:
                p.alignment = WD_ALIGN_PARAGRAPH.CENTER
            else:
                p.alignment = WD_ALIGN_PARAGRAPH.LEFT
        r = p.add_run(line)
        set_run_font(r, size_pt=13, bold=False, color_rgb=(0, 0, 0))

# Tiêu đề bảng phân công
title_p = doc.add_paragraph()
title_p.alignment = WD_ALIGN_PARAGRAPH.CENTER
title_p.paragraph_format.space_before = Pt(12)
title_p.paragraph_format.space_after = Pt(12)
r_title = title_p.add_run("BẢNG PHÂN CÔNG NHIỆM VỤ VÀ TỶ LỆ ĐÓNG GÓP")
set_run_font(r_title, size_pt=14, bold=True, color_rgb=(0, 0, 0))

# Ngắt trang trước bảng phân công
break_p = doc.add_paragraph()
r_br = break_p.add_run()
r_br.add_break(docx.enum.text.WD_BREAK.PAGE)

# Di chuyển các phần tử vào đúng vị trí sau Bảng 3
tbl3_el = body[tbl3_idx]
tbl3_el.addnext(break_p._p)
break_p._p.addnext(title_p._p)
title_p._p.addnext(new_tbl._tbl)
print("  -> Đã chèn xong Bảng Phân Công Nhiệm Vụ.")

# -------------------------------------------------------------
# BƯỚC 3: CHUẨN HÓA BULLET & CHUYỂN ĐỔI VĂN XUÔI HỌC THUẬT
# -------------------------------------------------------------
print("4. Chuẩn hóa Bullet và chuyển đổi văn xuôi...")

for p in doc.paragraphs:
    txt = p.text.strip()
    # Yêu cầu chức năng / Phi chức năng
    if txt.startswith("• Yêu cầu chức năng:") or txt.startswith("- Yêu cầu chức năng:"):
        p.text = "Về mặt yêu cầu chức năng, hệ thống bao quát 9 nhóm chức năng cốt lõi: (1) Quản lý tài khoản và cấu trúc đa ví tài sản; (2) Quản lý và hoạch định các dòng thu nhập; (3) Ghi nhận chi tiêu và bóc tách hóa đơn thông minh qua AI; (4) Hoạch định ngân sách đa phương pháp kết hợp hạn mức chi tiêu mỗi ngày thích ứng (DSA); (5) Quản lý hóa đơn định kỳ và dịch vụ đăng ký gia hạn; (6) Quản lý quỹ tích lũy mục tiêu và tối ưu hóa giải phóng nghĩa vụ nợ; (7) Báo cáo, phân tích tài chính đa chiều và trực quan hóa dòng tiền; (8) Dự báo tài chính dòng tiền và cảnh báo sớm rủi ro thâm hụt; (9) Hệ thống cú hích hành vi tài chính và hỗ trợ quyết định người dùng."
        for r in p.runs:
            set_run_font(r, size_pt=13)
        format_academic_paragraph(p)
        print("  -> Chuyển đổi P Yêu cầu chức năng sang văn xuôi.")
    elif txt.startswith("• Yêu cầu phi chức năng:") or txt.startswith("- Yêu cầu phi chức năng:"):
        p.text = "Về mặt yêu cầu phi chức năng, hệ thống bảo đảm tính toàn vẹn dữ liệu giao dịch theo chuẩn ACID với cơ chế khóa bi quan (Pessimistic Locking) trong thao tác đa ví; phản hồi giao diện tức thì theo mô hình Optimistic UI (độ trễ dưới 100ms); hỗ trợ xử lý ngoại tuyến (Offline-First) và đồng bộ hai chiều; pipeline thị giác AI bóc tách hóa đơn đáp ứng độ chính xác trên 90% kết hợp thuật toán đối soát kiểm toán số học ba tầng; bảo mật chuẩn Fintech với mã hóa AES-256 đối với dữ liệu nhạy cảm, băm mật khẩu Argon2id và luân chuyển khóa xác thực JWT; giao diện người dùng đáp ứng tiêu chuẩn tiếp cận WCAG 2.1 AA."
        for r in p.runs:
            set_run_font(r, size_pt=13)
        format_academic_paragraph(p)
        print("  -> Chuyển đổi P Yêu cầu phi chức năng sang văn xuôi.")
    # Các bullet trong 1.7.2, 2.2.3, 3.3.1
    elif any(txt.startswith(prefix) for prefix in [
        "–  Dữ liệu", "–  Xử lý", "–  Bóc tách", "–  Động cơ", "–  Bảng điều khiển", "–  Trực quan hóa", "–  Kế hoạch", "–  Khuyến nghị",
        "–  Kiểm tra tích số học", "–  Kiểm tra tổng lũy kế", "–  Xử lý phân nhánh",
        "• Giai đoạn 1:", "• Giai đoạn 2:", "• Giai đoạn 3:", "• Giai đoạn 4:", "• Giai đoạn 5:"
    ]):
        clean_txt = txt.lstrip('–—•- ').strip()
        p.text = "- " + clean_txt
        for r in p.runs:
            set_run_font(r, size_pt=13)
        format_bullet_paragraph(p)
        print(f"  -> Chuẩn hóa bullet 1/2 tab: {clean_txt[:40]}...")

# -------------------------------------------------------------
# BƯỚC 4: HỢP NHẤT CLASS DIAGRAM VÀO MỤC 3.3 (ẢNH + VĂN XUÔI)
# -------------------------------------------------------------
print("5. Hợp nhất Class Diagram vào Mục 3.3...")

def replace_with_image(p, img_path, width=None, height=None):
    p.text = ""
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p.paragraph_format.first_line_indent = Cm(0)
    p.paragraph_format.left_indent = Cm(0)
    p.paragraph_format.space_before = Pt(8)
    p.paragraph_format.space_after = Pt(4)
    r = p.add_run()
    if width and height:
        r.add_picture(img_path, width=width, height=height)
    elif width:
        r.add_picture(img_path, width=width)
    elif height:
        r.add_picture(img_path, height=height)
    else:
        r.add_picture(img_path, width=Inches(6.1))

for i, p in enumerate(doc.paragraphs):
    txt = p.text.strip()
    if "Dưới đây là sơ đồ chi tiết và bảng từ điển" in txt:
        p.text = "Dưới đây là sơ đồ tổng quan và các bản vẽ chi tiết độ phân giải cao, đi kèm luận giải học thuật chuyên sâu về cấu trúc hướng miền của các lớp thực thể phân bổ theo 6 Bounded Contexts:"
        for r in p.runs:
            set_run_font(r, size_pt=13)
        format_academic_paragraph(p)
    elif "[ KHUNG HÌNH ẢNH: SƠ ĐỒ KIẾN TRÚC TỔNG THỂ" in txt:
        replace_with_image(p, 'files/images/cd_overview.png', width=Inches(6.1))
        print("  -> Đã chèn ảnh cd_overview.png")
    elif "Bao gồm 3 lớp: User (Thực thể gốc" in txt:
        p.text = "Ngữ cảnh Định danh & Hồ sơ cá nhân (Identity & Profile Context) đóng vai trò là hạt nhân trung tâm (Core Domain) của hệ thống tài chính cá nhân, chịu trách nhiệm thiết lập danh tính duy nhất của người dùng (User) và quản trị toàn bộ các thuộc tính nền tảng chi phối hành vi dòng tiền. Thực thể gốc User quản lý thông tin tài khoản xác thực, mật khẩu mã hóa Argon2id, chu kỳ tài chính cá nhân (ngày nhận lương salaryDay, chu kỳ ngân sách budgetCycle), mức độ chấp nhận rủi ro (riskTolerance) và tiền tệ mặc định (defaultCurrency). Điểm nhấn kiến trúc cốt lõi của ngữ cảnh này là việc định nghĩa hai Đối tượng Giá trị bất biến (Immutable Value Objects) dùng chung cho toàn bộ hệ thống: (1) Money: đóng gói số tiền kiểu số thực chính xác cao (Decimal 18,2) cùng mã tiền tệ ISO-4217, ngăn chặn triệt để lỗi làm tròn số học và chứa các phương thức nghiệp vụ bất biến như add(), subtract(), multiply(), allocate(); (2) DateRange: đóng gói khoảng thời gian chu kỳ với ngày bắt đầu (startDate) và ngày kết thúc (endDate), bảo đảm quy tắc bất biến startDate luôn nhỏ hơn hoặc bằng endDate cùng các phương thức kiểm tra giao thoa includesDate() và overlapsWith()."
        for r in p.runs:
            set_run_font(r, size_pt=13)
        format_academic_paragraph(p)
    elif "[ KHUNG HÌNH ẢNH: CONTEXT 1:" in txt:
        replace_with_image(p, 'files/images/cd_context_1_identity.png', width=Inches(6.1))
        print("  -> Đã chèn ảnh cd_context_1_identity.png")
    elif "Bao gồm 7 lớp: Wallet (Lớp trừu tượng" in txt:
        p.text = "Ngữ cảnh Tài khoản & Đa ví cá nhân (Account & Multi-Wallet Context) quản lý toàn diện các kênh thanh toán và phương tiện lưu trữ tiền tệ của người dùng thông qua mô hình kế thừa hướng đối tượng với lớp cha trừu tượng Wallet và bốn lớp con chuyên biệt: CashWallet (Ví tiền mặt vật lý, kiểm soát theo chu kỳ rút tiền), BankAccountWallet (Tài khoản ngân hàng thanh toán, liên kết số tài khoản và định danh ngân hàng chuẩn NAPAS), CreditCardWallet (Thẻ tín dụng với hạn mức tín dụng creditLimit, ngày chốt sao kê statementClosingDate và ngày đến hạn thanh toán paymentDueDate để kiểm soát nợ quay vòng), và EWallet (Ví điện tử MoMo, ZaloPay, ShopeePay liên kết số điện thoại). Mọi thao tác biến động tài sản như nạp tiền (deposit), rút tiền (withdraw) hay chuyển tiền nội bộ giữa hai ví (InternalTransfer) đều bắt buộc tuân thủ nguyên lý giao dịch ngân hàng ACID với cơ chế khóa bi quan (Pessimistic Locking) trên CSDL, bảo đảm không bao giờ xảy ra tình trạng mất mát dữ liệu hoặc số dư bị âm ngoài hạn mức cho phép. Lớp thực thể BalanceAdjustment lưu trữ vết kiểm kê và lý do chênh lệch giữa số dư thực tế và số dư ghi nhận trên hệ thống."
        for r in p.runs:
            set_run_font(r, size_pt=13)
        format_academic_paragraph(p)
    elif "[ KHUNG HÌNH ẢNH: CONTEXT 2:" in txt:
        replace_with_image(p, 'files/images/cd_context_2_wallet.png', height=Inches(5.8))
        print("  -> Đã chèn ảnh cd_context_2_wallet.png")
    elif "Bao gồm 7 lớp: Transaction (Lớp giao dịch trừu tượng)" in txt:
        p.text = "Ngữ cảnh Giao dịch, AI Vision OCR & Danh mục (Transactions, AI OCR & Categories Context) chịu trách nhiệm kiểm soát toàn bộ dòng tiền biến động phát sinh hàng ngày. Trung tâm ngữ cảnh là lớp trừu tượng Transaction cùng hai phân lớp chuyên biệt: IncomeTransaction (Giao dịch thu nhập với cờ nhận diện thu nhập bất thường isWindfall) và ExpenseTransaction (Giao dịch chi tiêu liên kết danh mục chi và cờ đánh dấu chi phí thiết yếu isEssentialExpense). Kiến trúc nghiệp vụ nổi bật là mối quan hệ hợp thành chặt chẽ (Composition) giữa Hóa đơn bóc tách tự động (Receipt) và các dòng mặt hàng chi tiết (ReceiptItem): mỗi hóa đơn khi được xử lý qua pipeline thị giác AI Multimodal Vision OCR sẽ được kiểm toán tính toàn vẹn số học thông qua phương thức verifyMathematicalAudit() trước khi phê duyệt ghi nhận giao dịch. Cây danh mục Category được thiết kế theo cấu trúc phân cấp đệ quy Cha - Con (Parent-Child Tree) hỗ trợ phân loại linh hoạt, đồng thời tích hợp lớp AutoCategorizationRule để tự động gắn thẻ danh mục dựa trên từ khóa mô tả và mẫu giao dịch lặp lại. Lớp thực thể Subscription phụ trách giám sát và cảnh báo trước các cam kết thanh toán dịch vụ định kỳ (Netflix, Spotify, iCloud, tiền cước viễn thông)."
        for r in p.runs:
            set_run_font(r, size_pt=13)
        format_academic_paragraph(p)
    elif "[ KHUNG HÌNH ẢNH: CONTEXT 3:" in txt:
        replace_with_image(p, 'files/images/cd_context_3_transactions.png', height=Inches(5.8))
        print("  -> Đã chèn ảnh cd_context_3_transactions.png")
    elif "Bao gồm 3 lớp: BudgetPlan" in txt:
        p.text = "Ngữ cảnh Ngân sách đa phương pháp & DSA (Budgeting & Adaptive DSA Context) hiện thực hóa các chiến lược kiểm soát tài chính cá nhân khoa học từ các trường phái quốc tế (Quy tắc 50/30/20, Phương pháp Ngân sách Cân bằng Zero-Based Budgeting và Phong cách Quản lý Sổ tay Kakeibo Nhật Bản). Thực thể Aggregate Root Budget định nghĩa chu kỳ ngân sách và liên kết hợp thành với tập hợp các hạn mức phân bổ chi tiết BudgetAllocation theo từng nhóm danh mục. Đột phá công nghệ tại ngữ cảnh này là thực thể tính toán Hạn mức chi tiêu mỗi ngày thích ứng (DailySpendingAllowance - DSA). Thay vì áp đặt một con số cứng nhắc trong suốt tháng, thực thể DSA sở hữu các phương thức tính toán thông minh calculateDSA(), computeSpendingVelocity() và estimateOutOfMoneyDate(), liên tục đo lường tốc độ chi tiêu thực tế của người dùng và tự động tái phân bổ hạn mức chi tiêu của những ngày còn lại trong chu kỳ khi có phát sinh chi tiêu đột biến, giúp người dùng duy trì kỷ luật dòng tiền mà không rơi vào trạng thái căng thẳng tâm lý."
        for r in p.runs:
            set_run_font(r, size_pt=13)
        format_academic_paragraph(p)
    elif "[ KHUNG HÌNH ẢNH: CONTEXT 4:" in txt:
        replace_with_image(p, 'files/images/cd_context_4_budgeting.png', width=Inches(6.1))
        print("  -> Đã chèn ảnh cd_context_4_budgeting.png")
    elif "Bao gồm 4 lớp: FinancialGoal" in txt:
        p.text = "Ngữ cảnh Quỹ tích lũy & Tối ưu hóa Nợ cá nhân (Personal Debt & Financial Goals Context) giải quyết bài toán cân bằng dài hạn giữa nghĩa vụ nợ phải trả và mục tiêu tích lũy tài sản. Về mặt tích lũy, thực thể FinancialGoal và lớp con kế thừa chuyên biệt EmergencyFundGoal kiểm soát tiến độ tiết kiệm thông qua các khoản đóng góp GoalContribution, trong đó Quỹ dự phòng khẩn cấp được thiết lập nguyên tắc bảo vệ nghiêm ngặt nhằm duy trì số dư tối thiểu tương đương 3 đến 6 tháng sinh hoạt phí cơ bản. Về mặt quản trị nợ, thực thể DebtAccount quản lý các tài khoản nợ vay cá nhân (khoản vay trả góp, thẻ tín dụng, nợ người thân) với lãi suất và ngày đến hạn. Kế hoạch thanh toán nợ DebtPaymentPlan liên kết với lịch trình trả nợ DebtRepaymentScheduleItem và các giao dịch thanh toán DebtPaymentTransaction, được trang bị hai giải thuật đối sánh mô phỏng: simulateSnowball() (Chiến lược quả cầu tuyết - ưu tiên trả nhanh các khoản nợ nhỏ nhất để tạo động lực tâm lý) và simulateAvalanche() (Chiến lược tuyết lở - ưu tiên dồn tiền trả khoản nợ có lãi suất cao nhất nhằm tối thiểu hóa tổng chi phí lãi vay), giúp người dùng có cái nhìn toàn diện để lựa chọn phương án thanh toán tối ưu."
        for r in p.runs:
            set_run_font(r, size_pt=13)
        format_academic_paragraph(p)
    elif "[ KHUNG HÌNH ẢNH: CONTEXT 5:" in txt:
        replace_with_image(p, 'files/images/cd_context_5_debt_goals.png', height=Inches(5.8))
        print("  -> Đã chèn ảnh cd_context_5_debt_goals.png")
    elif "Bao gồm thực thể trung tâm FinancialNudge" in txt:
        p.text = "Ngữ cảnh Cú hích hành vi & Hỗ trợ quyết định Thaler (Decision Support & Behavioral Nudge Context) hiện thực hóa các nguyên lý của Kinh tế học Hành vi và Lý thuyết Cú hích (Nudge Theory) do Giáo sư Richard Thaler (Nobel Kinh tế 2017) khởi xướng. Thực thể trung tâm FinancialNudge đóng vai trò là một can thiệp kiến trúc lựa chọn phi cưỡng chế, sở hữu các thuộc tính minh bạch giải trình rationaleExplanation (lý do cụ thể vì sao hệ thống đưa ra cú hích), cờ xác nhận có ý thức của người dùng isApprovedByUser và mốc thời gian trì hoãn coolingOffUntil. Thông qua các phương thức hành vi phong phú applyCoolingOff() và confirmHumanAction(), hệ thống thiết lập khoảng trễ tâm lý bắt buộc từ 24 đến 48 giờ đối với các quyết định chi tiêu lớn vượt ngưỡng an toàn hoặc các khoản mua sắm bốc đồng ngoài kế hoạch, đồng thời tái định vị các tài khoản tâm lý (Mental Accounting) giúp người dùng tỉnh táo nhận thức được cái giá cơ hội của quyết định chi tiêu trước khi thực sự xuất tiền."
        for r in p.runs:
            set_run_font(r, size_pt=13)
        format_academic_paragraph(p)
    elif "[ KHUNG HÌNH ẢNH: CONTEXT 6:" in txt:
        replace_with_image(p, 'files/images/cd_context_6_nudge.png', width=Inches(6.1))
        print("  -> Đã chèn ảnh cd_context_6_nudge.png")
    elif txt.startswith("Hình 3.") and any(f"Context {n}" in txt for n in range(1, 7)) or "Hình 3.11" in txt:
        p.alignment = WD_ALIGN_PARAGRAPH.CENTER
        p.paragraph_format.first_line_indent = Cm(0)
        p.paragraph_format.left_indent = Cm(0)
        p.paragraph_format.space_before = Pt(4)
        p.paragraph_format.space_after = Pt(8)
        for r in p.runs:
            set_run_font(r, size_pt=13, bold=False, italic=True)

# -------------------------------------------------------------
# BƯỚC 5: SỬA TIÊU ĐỀ MỤC 3.4 & DỌN DẸP PLACEHOLDER TRONG CHƯƠNG 3
# -------------------------------------------------------------
print("6. Sửa tiêu đề Mục 3.4 và dọn dẹp placeholder trong Chương 3...")

# Sửa tiêu đề P334 (tìm bằng text '3.4' và 'động học')
for p in doc.paragraphs:
    if "3.4" in p.text and "động học" in p.text.lower():
        for r in p.runs:
            if "động học" in r.text.lower() or "3.4" in r.text:
                r.text = "3.4 Thiết kế tương tác và sơ đồ tuần tự hệ thống (Sequence Diagrams)"
                set_run_font(r, size_pt=13, bold=True)
        p.paragraph_format.space_before = Pt(12)
        p.paragraph_format.space_after = Pt(6)
        print("  -> Đã đổi tên Mục 3.4 thành công (bảo toàn Bookmark).")
        break

# Dọn dẹp các đoạn stub placeholder
elements_to_delete = []
for p in doc.paragraphs:
    txt = p.text.strip()
    if txt.startswith(("• SD-01:", "• SD-02:", "• SD-03:", "• SD-04:", "• SD-05:", "- SD-01:", "- SD-02:", "- SD-03:", "- SD-04:", "- SD-05:")):
        elements_to_delete.append(p._p)
    elif "Thiết kế động học tập trung vào việc mô hình hóa các luồng tương tác" in txt:
        elements_to_delete.append(p._p)
    elif "Thiết kế cơ sở dữ liệu chuyển hóa các thực thể miền thành lược đồ quan hệ vật lý đạt chuẩn" in txt:
        elements_to_delete.append(p._p)
    elif "Thiết kế kiến trúc phần mềm xác lập mô hình Clean Architecture 4 tầng" in txt:
        elements_to_delete.append(p._p)

print(f"  -> Số lượng đoạn stub cần dọn dẹp: {len(elements_to_delete)}")
for el in elements_to_delete:
    parent = el.getparent()
    if parent is not None:
        parent.remove(el)

# -------------------------------------------------------------
# BƯỚC 6: BỔ SUNG MỤC 2.5 CÔNG NGHỆ DỰ KIẾN VÀO CHƯƠNG 2
# -------------------------------------------------------------
print("7. Bổ sung Mục 2.5 Công nghệ dự kiến vào Chương 2...")

# Tìm paragraph CHƯƠNG 3
target_ch3_p = None
for p in doc.paragraphs:
    if "CHƯƠNG 3: PHÂN TÍCH VÀ THIẾT KẾ HỆ THỐNG" in p.text:
        target_ch3_p = p
        break

if target_ch3_p:
    print("  -> Tìm thấy vị trí trước Chương 3.")
    
    h2_5 = doc.add_paragraph()
    h2_5.style = doc.styles['Heading 2']
    h2_5.alignment = WD_ALIGN_PARAGRAPH.LEFT
    h2_5.paragraph_format.space_before = Pt(12)
    h2_5.paragraph_format.space_after = Pt(6)
    r = h2_5.add_run("2.5 Định hướng và Đề xuất Hệ sinh thái Công nghệ Triển khai Dự án")
    set_run_font(r, size_pt=13, bold=True)

    def create_academic_p(text, bold_prefix=None):
        p = doc.add_paragraph()
        format_academic_paragraph(p)
        if bold_prefix:
            r_pre = p.add_run(bold_prefix)
            set_run_font(r_pre, size_pt=13, bold=True)
        r_body = p.add_run(text)
        set_run_font(r_body, size_pt=13, bold=False)
        return p

    p2_5_intro = create_academic_p(
        "Trên cơ sở các nền tảng lý thuyết về Kinh tế học Hành vi, Thiết kế Hướng miền (DDD), Kiến trúc Clean Architecture và công nghệ Trí tuệ Nhân tạo thị giác đa phương thức đã được phân tích ở các mục trước, đề tài xác lập một hệ sinh thái công nghệ sơ bộ làm định hướng cho quá trình hiện thực hóa phần mềm. Đây là bản đề xuất khung kiến trúc công nghệ ban đầu phục vụ giai đoạn phân tích thiết kế, có thể được tinh chỉnh, mở rộng hoặc bổ sung trong quá trình thực nghiệm kỹ thuật ở Chương 4 nhằm đạt được sự cân bằng tối ưu giữa hiệu năng vận hành, tính ổn định và chi phí hạ tầng."
    )

    p2_5_client = create_academic_p(
        "Đề xuất giải pháp xây dựng ứng dụng di động đa nền tảng sử dụng React Native hoặc Flutter nhằm bảo đảm trải nghiệm người dùng nhất quán trên cả hai hệ điều hành Android và iOS, kết hợp với giao diện Web quản trị phát triển trên nền tảng Next.js/React. Kiến trúc phía máy khách áp dụng triệt để mô hình phản hồi tức thì Optimistic UI với mục tiêu triệt tiêu độ trễ tương tác (dưới 100ms) khi ghi chép giao dịch, tích hợp cơ chế Offline-First với bộ lưu trữ cục bộ (SQLite / WatermelonDB) cho phép người dùng nhập liệu thu chi bình thường ngay cả khi không có kết nối Internet và tự động đồng bộ hai chiều khi mạng được khôi phục.",
        bold_prefix="Nền tảng Giao diện Đa nền tảng và Trải nghiệm Người dùng (Cross-Platform Client): "
    )

    p2_5_server = create_academic_p(
        "Dịch vụ máy chủ ứng dụng được định hướng xây dựng trên nền tảng ASP.NET Core Web API (.NET 8) hoặc Node.js (TypeScript / NestJS), áp dụng nghiêm ngặt mô hình Clean Architecture 4 tầng phân tách độc lập (Domain, Application, Infrastructure, Presentation) và mẫu thiết kế CQRS (Command Query Responsibility Segregation). Kiến trúc phân tách này bảo đảm toàn bộ các quy tắc nghiệp vụ tài chính và các thuật toán miền độc lập hoàn toàn với framework và cơ sở dữ liệu, tạo điều kiện thuận lợi cho việc kiểm thử tự động và mở rộng tính năng về lâu dài.",
        bold_prefix="Dịch vụ Máy chủ Ứng dụng và Xử lý Nghiệp vụ (Backend Application Server): "
    )

    p2_5_db = create_academic_p(
        "Hệ thống định hướng sử dụng mô hình lưu trữ đa hệ quản trị (Polyglot Persistence): Hệ quản trị CSDL quan hệ Microsoft SQL Server hoặc PostgreSQL đạt chuẩn Dạng chuẩn 3 (3NF) được dùng làm kho lưu trữ chính cho các thực thể giao dịch, tài khoản đa ví và kế hoạch ngân sách, bảo đảm tính toàn vẹn ACID tuyệt đối thông qua kỹ thuật khóa bi quan (Pessimistic Locking - UPDLOCK, ROWLOCK). Bộ nhớ đệm Redis Cache được tích hợp để lưu trữ tạm thời hạn mức chi tiêu mỗi ngày thích ứng (DSA), quản lý phiên làm việc JWT và kiểm soát tần suất truy vấn API (Rate Limiting). Dữ liệu nhị phân ảnh chụp hóa đơn gốc được lưu trữ phân tán trên dịch vụ Object Storage tương thích S3 / MinIO.",
        bold_prefix="Hệ thống Cơ sở Dữ liệu Toàn diện và Lưu trữ Phân tán (Universal / Polyglot Database): "
    )

    p2_5_ai = create_academic_p(
        "Tích hợp mô hình AI thị giác đa phương thức (Google Gemini Flash Multimodal API hoặc OpenAI GPT-4o Vision API) phục vụ bóc tách tự động dữ liệu hóa đơn mua sắm. Luồng bóc tách được điều phối theo kiến trúc xử lý bất đồng bộ thông qua hàng đợi thông điệp (Message Queue: RabbitMQ hoặc BullMQ) tuân thủ chuẩn HTTP 202 Accepted, tách biệt thời gian chờ suy luận của mô hình AI khỏi trải nghiệm người dùng tức thì. Sau khi bóc tách, thuật toán Hậu kiểm toán số học ba tầng (Mathematical Audit) sẽ tự động kiểm tra tính cân bằng của phương trình kế toán trước khi hệ thống lưu vết giao dịch.",
        bold_prefix="Pipeline Trí tuệ Nhân tạo Thị giác và Hàng đợi Bất đồng bộ (AI Multimodal Vision Pipeline): "
    )

    p2_5_sec = create_academic_p(
        "Triển khai cơ chế mã hóa dữ liệu nhạy cảm ở trạng thái lưu trữ (Data at Rest) bằng thuật toán mã hóa khóa đối xứng AES-256-GCM. Quản lý danh tính và phiên người dùng thông qua cơ chế luân chuyển mã xác thực kép JWT (Short-lived Access Token thời hạn 15 phút kết hợp Sliding Refresh Token lưu trong HttpOnly Cookie/Secure Storage). Mật khẩu người dùng được băm một chiều bằng giải thuật tiên tiến Argon2id có khả năng kháng tấn công phần cứng GPU. Toàn bộ các cổng giao tiếp API đều được kiểm soát bởi hàng rào phòng thủ bảo mật tuân thủ nghiêm ngặt chuẩn OWASP Top 10 API Security.",
        bold_prefix="Tiêu chuẩn An ninh Thông tin và Mật mã học Fintech (Fintech Security Standards): "
    )

    p2_5_note = create_academic_p(
        "Nhìn chung, hệ sinh thái công nghệ trên được đề xuất dựa trên các tiêu chí tương thích cao với phương pháp luận Domain-Driven Design, đáp ứng các yêu cầu khắt khe về tính toàn vẹn dữ liệu tài chính và năng lực mở rộng xử lý AI. Trong các giai đoạn tiếp theo của đề tài, các thành phần công nghệ này sẽ được hiện thực hóa và đánh giá thực nghiệm định lượng về hiệu năng, độ trễ và độ tin cậy để đưa ra lựa chọn chính thức."
    )

    # Chèn vào trước Chương 3
    target_xml = target_ch3_p._p
    for p_item in [h2_5, p2_5_intro, p2_5_client, p2_5_server, p2_5_db, p2_5_ai, p2_5_sec, p2_5_note]:
        target_xml.addprevious(p_item._p)
    print("  -> Đã chèn xong toàn bộ Mục 2.5.")

# -------------------------------------------------------------
# LƯU FILE HOÀN TẤT
# -------------------------------------------------------------
doc.save(src_file)
print(f"=== ĐÃ HOÀN TẤT VÀ LƯU THÀNH CÔNG VÀO TỆP: {src_file} ===")
