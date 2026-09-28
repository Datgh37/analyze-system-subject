import docx
from docx import Document
from docx.shared import Inches, Pt, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.enum.table import WD_TABLE_ALIGNMENT
from docx.oxml import parse_xml
from docx.oxml.ns import nsdecls, qn
import html

bookmark_id_counter = 0

def add_bookmark(paragraph, bookmark_name):
    global bookmark_id_counter
    bookmark_id_counter += 1
    b_id = str(bookmark_id_counter)
    
    start = parse_xml(f'<w:bookmarkStart {nsdecls("w")} w:id="{b_id}" w:name="{bookmark_name}"/>')
    end = parse_xml(f'<w:bookmarkEnd {nsdecls("w")} w:id="{b_id}"/>')
    
    paragraph._p.insert(0, start)
    paragraph._p.append(end)

def add_persona_links_to_cell(cell, persona_str):
    p = cell.paragraphs[0]
    p.text = ""
    parts = [x.strip() for x in persona_str.split(",")]
    for idx, num in enumerate(parts):
        if idx > 0:
            r_comma = p.add_run(", ")
            r_comma.font.name = "Cambria"
            r_comma.font.size = Pt(10.5)
            r_comma.font.color.rgb = RGBColor(0x2D, 0x37, 0x48)
        
        anchor = f"persona_{num}"
        hyperlink = parse_xml(f'<w:hyperlink {nsdecls("w")} w:anchor="{anchor}" w:history="1"/>')
        r = parse_xml(f'''
            <w:r {nsdecls("w")}>
                <w:rPr>
                    <w:rFonts w:ascii="Cambria" w:hAnsi="Cambria"/>
                    <w:b/>
                    <w:color w:val="2B4C7E"/>
                    <w:u w:val="single"/>
                    <w:sz w:val="21"/>
                </w:rPr>
                <w:t>{num}</w:t>
            </w:r>
        ''')
        hyperlink.append(r)
        p._p.append(hyperlink)

def set_cell_background(cell, fill_hex):
    tcPr = cell._tc.get_or_add_tcPr()
    shd = parse_xml(f'<w:shd {nsdecls("w")} w:fill="{fill_hex}"/>')
    tcPr.append(shd)

def set_cell_margins(cell, top=120, bottom=120, left=150, right=150):
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

def set_table_borders(table, color="B0C4DE", sz="4", val="single"):
    tblPr = table._tbl.tblPr
    borders = parse_xml(f'''
        <w:tblBorders {nsdecls("w")}>
            <w:top w:val="{val}" w:sz="{sz}" w:space="0" w:color="{color}"/>
            <w:left w:val="{val}" w:sz="{sz}" w:space="0" w:color="{color}"/>
            <w:bottom w:val="{val}" w:sz="{sz}" w:space="0" w:color="{color}"/>
            <w:right w:val="{val}" w:sz="{sz}" w:space="0" w:color="{color}"/>
            <w:insideH w:val="{val}" w:sz="{sz}" w:space="0" w:color="{color}"/>
            <w:insideV w:val="{val}" w:sz="{sz}" w:space="0" w:color="{color}"/>
        </w:tblBorders>
    ''')
    tblPr.append(borders)

def style_table(table, col_widths, headers, rows_data, is_persona_col=None):
    table.alignment = WD_TABLE_ALIGNMENT.CENTER
    table.autofit = False
    set_table_borders(table)
    
    # Header row
    hdr_cells = table.rows[0].cells
    for i, header_text in enumerate(headers):
        cell = hdr_cells[i]
        cell.width = col_widths[i]
        set_cell_background(cell, "2B4C7E")
        set_cell_margins(cell, top=140, bottom=140, left=150, right=150)
        p = cell.paragraphs[0]
        p.alignment = WD_ALIGN_PARAGRAPH.CENTER
        p.paragraph_format.space_before = Pt(0)
        p.paragraph_format.space_after = Pt(0)
        r = p.add_run(header_text)
        r.font.name = "Cambria"
        r.font.size = Pt(11)
        r.font.bold = True
        r.font.color.rgb = RGBColor(0xFF, 0xFF, 0xFF)
    
    trPr = table.rows[0]._tr.get_or_add_trPr()
    trPr.append(parse_xml(f'<w:tblHeader {nsdecls("w")}/>'))
    
    # Data rows
    for ri, row_data in enumerate(rows_data):
        row = table.rows[ri + 1]
        r_trPr = row._tr.get_or_add_trPr()
        r_trPr.append(parse_xml(f'<w:cantSplit {nsdecls("w")}/>'))
        
        bg_color = "F7FAFC" if ri % 2 == 1 else "FFFFFF"
        for ci, val in enumerate(row_data):
            cell = row.cells[ci]
            cell.width = col_widths[ci]
            set_cell_background(cell, bg_color)
            set_cell_margins(cell, top=100, bottom=100, left=140, right=140)
            p = cell.paragraphs[0]
            p.paragraph_format.space_before = Pt(0)
            p.paragraph_format.space_after = Pt(0)
            p.paragraph_format.line_spacing = 1.15
            
            if is_persona_col is not None and ci == is_persona_col:
                p.alignment = WD_ALIGN_PARAGRAPH.CENTER
                add_persona_links_to_cell(cell, str(val))
            else:
                if ci == 0 and len(str(val)) <= 5:
                    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
                else:
                    p.alignment = WD_ALIGN_PARAGRAPH.LEFT
                
                r = p.add_run(str(val))
                r.font.name = "Cambria"
                r.font.size = Pt(10.5)
                r.font.color.rgb = RGBColor(0x2D, 0x37, 0x48)

def add_native_word_toc(doc, title="MỤC LỤC TỰ ĐỘNG (TABLE OF CONTENTS)"):
    # Title paragraph
    p_title = doc.add_paragraph()
    p_title.paragraph_format.space_before = Pt(14)
    p_title.paragraph_format.space_after = Pt(8)
    p_title.paragraph_format.keep_with_next = True
    r = p_title.add_run(title)
    r.font.name = "Cambria"
    r.font.size = Pt(14)
    r.font.bold = True
    r.font.color.rgb = RGBColor(0x1A, 0x36, 0x5D)
    add_bookmark(p_title, "muc_luc")
    
    # Instruction / description
    p_desc = doc.add_paragraph()
    p_desc.paragraph_format.space_before = Pt(0)
    p_desc.paragraph_format.space_after = Pt(6)
    r_desc = p_desc.add_run("Mục lục được tạo bằng trường tham chiếu tự động (Word TOC Field Code). Bạn có thể cập nhật mục lục bất kỳ lúc nào bằng cách nhấp chuột phải chọn 'Update Field' (hoặc nhấn F9) trong Microsoft Word.")
    r_desc.font.name = "Cambria"
    r_desc.font.size = Pt(11)
    r_desc.font.italic = True
    r_desc.font.color.rgb = RGBColor(0x71, 0x80, 0x96)

    # Paragraph containing the native TOC field code: { TOC \o "1-3" \h \z \u }
    p_fld = doc.add_paragraph()
    p_fld.paragraph_format.space_before = Pt(4)
    p_fld.paragraph_format.space_after = Pt(14)
    
    # Field begin
    r1 = p_fld.add_run()
    fldChar1 = parse_xml(r'<w:fldChar %s w:fldCharType="begin"/>' % nsdecls('w'))
    r1._r.append(fldChar1)
    
    # Field instruction
    r2 = p_fld.add_run()
    instrText = parse_xml(r'<w:instrText %s xml:space="preserve"> TOC \o "1-3" \h \z \u </w:instrText>' % nsdecls('w'))
    r2._r.append(instrText)
    
    # Field separator
    r3 = p_fld.add_run()
    fldChar2 = parse_xml(r'<w:fldChar %s w:fldCharType="separate"/>' % nsdecls('w'))
    r3._r.append(fldChar2)
    
    # Placeholder text inside TOC
    r4 = p_fld.add_run("[Nhấp chuột phải vào đây và chọn 'Update Field' hoặc nhấn F9 trong Microsoft Word để hiển thị Mục lục]")
    r4.font.name = "Cambria"
    r4.font.italic = True
    r4.font.size = Pt(11)
    r4.font.color.rgb = RGBColor(0x2B, 0x4C, 0x7E)
    
    # Field end
    r5 = p_fld.add_run()
    fldChar3 = parse_xml(r'<w:fldChar %s w:fldCharType="end"/>' % nsdecls('w'))
    r5._r.append(fldChar3)

def set_update_fields_on_open(doc):
    element = doc.settings.element
    update_fields = parse_xml(f'<w:updateFields {nsdecls("w")} w:val="true"/>')
    element.append(update_fields)

def create_full_report_with_native_headings(output_path):
    global bookmark_id_counter
    bookmark_id_counter = 0
    
    doc = Document()
    set_update_fields_on_open(doc)
    
    # Page setup
    sec = doc.sections[0]
    sec.page_width = Inches(8.5)
    sec.page_height = Inches(11.0)
    sec.top_margin = Inches(1.0)
    sec.bottom_margin = Inches(1.0)
    sec.left_margin = Inches(1.0)
    sec.right_margin = Inches(1.0)
    
    # Normal Style
    style_normal = doc.styles['Normal']
    style_normal.font.name = 'Cambria'
    style_normal.font.size = Pt(12)
    style_normal.font.color.rgb = RGBColor(0x2D, 0x37, 0x48)
    
    # Configure Heading 1 Style
    h1_style = doc.styles['Heading 1']
    h1_style.font.name = 'Cambria'
    h1_style.font.size = Pt(14)
    h1_style.font.bold = True
    h1_style.font.color.rgb = RGBColor(0x1A, 0x36, 0x5D)
    h1_style.paragraph_format.space_before = Pt(16)
    h1_style.paragraph_format.space_after = Pt(6)
    h1_style.paragraph_format.keep_with_next = True

    # Configure Heading 2 Style
    h2_style = doc.styles['Heading 2']
    h2_style.font.name = 'Cambria'
    h2_style.font.size = Pt(13)
    h2_style.font.bold = True
    h2_style.font.color.rgb = RGBColor(0x2B, 0x4C, 0x7E)
    h2_style.paragraph_format.space_before = Pt(12)
    h2_style.paragraph_format.space_after = Pt(4)
    h2_style.paragraph_format.keep_with_next = True

    # Configure Heading 3 Style
    h3_style = doc.styles['Heading 3']
    h3_style.font.name = 'Cambria'
    h3_style.font.size = Pt(12)
    h3_style.font.bold = True
    h3_style.font.color.rgb = RGBColor(0x1F, 0x4E, 0x5F)
    h3_style.paragraph_format.space_before = Pt(8)
    h3_style.paragraph_format.space_after = Pt(3)
    h3_style.paragraph_format.keep_with_next = True

    def add_title(text, bookmark=None):
        p = doc.add_paragraph()
        p.alignment = WD_ALIGN_PARAGRAPH.CENTER
        p.paragraph_format.space_before = Pt(0)
        p.paragraph_format.space_after = Pt(4)
        run = p.add_run(text)
        run.font.name = 'Cambria'
        run.font.size = Pt(18)
        run.font.bold = True
        run.font.color.rgb = RGBColor(0x1A, 0x36, 0x5D)
        if bookmark:
            add_bookmark(p, bookmark)
        return p

    def add_subtitle(text):
        p = doc.add_paragraph()
        p.alignment = WD_ALIGN_PARAGRAPH.CENTER
        p.paragraph_format.space_before = Pt(2)
        p.paragraph_format.space_after = Pt(8)
        run = p.add_run(text)
        run.font.name = 'Cambria'
        run.font.size = Pt(13)
        run.font.bold = True
        run.font.color.rgb = RGBColor(0x2B, 0x4C, 0x7E)
        return p

    def add_group_info(text):
        p = doc.add_paragraph()
        p.alignment = WD_ALIGN_PARAGRAPH.CENTER
        p.paragraph_format.space_before = Pt(0)
        p.paragraph_format.space_after = Pt(10)
        run = p.add_run(text)
        run.font.name = 'Cambria'
        run.font.size = Pt(12)
        run.font.bold = True
        run.font.italic = True
        run.font.color.rgb = RGBColor(0x4A, 0x55, 0x68)
        return p

    def add_h1(text, bookmark=None):
        p = doc.add_heading(text, level=1)
        pPr = p._p.get_or_add_pPr()
        outlineLvl = parse_xml(f'<w:outlineLvl {nsdecls("w")} w:val="0"/>')
        pPr.append(outlineLvl)
        if bookmark:
            add_bookmark(p, bookmark)
        return p

    def add_h2(text, bookmark=None):
        p = doc.add_heading(text, level=2)
        pPr = p._p.get_or_add_pPr()
        outlineLvl = parse_xml(f'<w:outlineLvl {nsdecls("w")} w:val="1"/>')
        pPr.append(outlineLvl)
        if bookmark:
            add_bookmark(p, bookmark)
        return p

    def add_h3(text, bookmark=None):
        p = doc.add_heading(text, level=3)
        pPr = p._p.get_or_add_pPr()
        outlineLvl = parse_xml(f'<w:outlineLvl {nsdecls("w")} w:val="2"/>')
        pPr.append(outlineLvl)
        if bookmark:
            add_bookmark(p, bookmark)
        return p

    def add_p(text, bold_prefix="", italic_prefix=False):
        p = doc.add_paragraph()
        p.paragraph_format.space_before = Pt(0)
        p.paragraph_format.space_after = Pt(4)
        p.paragraph_format.line_spacing = 1.15
        if bold_prefix:
            run_b = p.add_run(bold_prefix)
            run_b.font.name = 'Cambria'
            run_b.font.size = Pt(12)
            run_b.font.bold = True
            run_b.font.italic = italic_prefix
            run_b.font.color.rgb = RGBColor(0x1A, 0x36, 0x5D)
        if text:
            run_t = p.add_run(text)
            run_t.font.name = 'Cambria'
            run_t.font.size = Pt(12)
            run_t.font.color.rgb = RGBColor(0x2D, 0x37, 0x48)
        return p

    def add_bullet(text, bold_prefix="", italic_prefix=False):
        p = doc.add_paragraph(style='List Paragraph')
        p.paragraph_format.space_before = Pt(0)
        p.paragraph_format.space_after = Pt(3)
        p.paragraph_format.line_spacing = 1.15
        
        r_dot = p.add_run("•  ")
        r_dot.font.name = 'Cambria'
        r_dot.font.size = Pt(12)
        r_dot.font.bold = True
        r_dot.font.color.rgb = RGBColor(0x2B, 0x4C, 0x7E)

        if bold_prefix:
            run_b = p.add_run(bold_prefix)
            run_b.font.name = 'Cambria'
            run_b.font.size = Pt(12)
            run_b.font.bold = True
            run_b.font.italic = italic_prefix
            run_b.font.color.rgb = RGBColor(0x1A, 0x36, 0x5D)
        if text:
            run_t = p.add_run(text)
            run_t.font.name = 'Cambria'
            run_t.font.size = Pt(12)
            run_t.font.color.rgb = RGBColor(0x2D, 0x37, 0x48)
        return p

    # ==================== DOCUMENT HEADER ====================
    add_title("BÁO CÁO LAB 1 – KHẢO SÁT HIỆN TRẠNG", bookmark="doc_title")
    add_subtitle("ĐỀ TÀI: HỆ THỐNG QUẢN LÝ CHI TIÊU & LẬP NGÂN SÁCH CÁ NHÂN / NHÓM\n(SMART EXPENSE & BUDGET MANAGER)")
    add_group_info("Nhóm thực hiện: Duo No Pro")

    # Team table
    tbl_team = doc.add_table(rows=3, cols=4)
    tbl_team_widths = [Inches(0.6), Inches(2.8), Inches(1.8), Inches(1.3)]
    team_headers = ["STT", "Họ Tên & MSSV", "Khả năng chuyên môn", "Ghi chú"]
    team_rows = [
        ["1", "Trịnh Thành Đạt – 1923050149", "", ""],
        ["2", "Trần Trọng Đại – 1923050745", "", ""]
    ]
    style_table(tbl_team, tbl_team_widths, team_headers, team_rows)
    doc.add_paragraph().paragraph_format.space_after = Pt(4)

    # ==================== NATIVE DYNAMIC TABLE OF CONTENTS ====================
    add_native_word_toc(doc, title="MỤC LỤC TỰ ĐỘNG (TABLE OF CONTENTS)")

    doc.add_page_break()

    # ----------------------------------------------------------------------------------
    # MỤC I. PHÂN TÍCH CƠ CẤU TỔ CHỨC VÀ PHÂN QUYỀN NHÓM
    # ----------------------------------------------------------------------------------
    add_h1("I. PHÂN TÍCH CƠ CẤU TỔ CHỨC VÀ PHÂN QUYỀN NHÓM", bookmark="sec_1")
    
    add_h2("1. Cơ cấu tổ chức của một nhóm chi tiêu thực tế", bookmark="sec_1_1")
    add_bullet(" Khởi tạo không gian nhóm, cấu hình tỷ lệ đóng góp quỹ, duyệt thành viên, thiết lập quy tắc chia tiền và chốt quyết toán kỳ (Settle Up).", bold_prefix="Trưởng nhóm (Group Admin):", italic_prefix=True)
    add_bullet(" Người quản lý quỹ chung hoặc đứng ra ứng tiền túi chi hộ cho tập thể, nộp hóa đơn chứng từ.", bold_prefix="Thủ quỹ / Người chi trả (Treasurer / Primary Payer):", italic_prefix=True)
    add_bullet(" Đóng góp quỹ, tham gia chia tiền, nộp yêu cầu hoàn ứng (Reimbursement) và thanh toán nợ.", bold_prefix="Thành viên nhóm (Group Member / Contributor):", italic_prefix=True)
    add_bullet(" Con cái trong gia đình hoặc thành viên giám sát quỹ (chỉ có quyền xem báo cáo thu chi, không được chỉnh sửa).", bold_prefix="Người quan sát / Thụ hưởng (Viewer / Auditor / Beneficiary):", italic_prefix=True)

    add_h2("2. Cơ cấu vận hành nền tảng hệ thống", bookmark="sec_1_2")
    add_bullet(" Giám sát hạ tầng, chuẩn hóa danh mục tài chính toàn cục, audit log bảo mật.", bold_prefix="Quản trị viên hệ thống (System Administrator):", italic_prefix=True)
    add_bullet(" Cung cấp API sinh mã QR thanh toán động và xác thực giao dịch chuyển khoản.", bold_prefix="Đối tác Cổng thanh toán (VietQR / Banking Partner):", italic_prefix=True)
    add_bullet(" Bóc tách dữ liệu hóa đơn đa phương thức (Multimodal Vision).", bold_prefix="Dịch vụ Trí tuệ nhân tạo (AI OCR Engine):", italic_prefix=True)

    add_h2("3. Phân định 4 Ngữ cảnh Giới hạn (Bounded Contexts)", bookmark="sec_1_3")
    add_bullet(" Ghi nhận chi tiêu, danh mục, hóa đơn đính kèm.", bold_prefix="Expense Context:", italic_prefix=True)
    add_bullet(" Quỹ chung, quản lý thành viên, đồ thị cấn trừ nợ chéo.", bold_prefix="Group Settlement Context:", italic_prefix=True)
    add_bullet(" Hạn mức chu kỳ, tốc độ tiêu tiền (Spending Velocity).", bold_prefix="Budgeting Context:", italic_prefix=True)
    add_bullet(" Định danh người dùng, xác thực và phân quyền.", bold_prefix="Identity & Access Context:", italic_prefix=True)

    # ----------------------------------------------------------------------------------
    # MỤC II. QUY TRÌNH NGHIỆP VỤ HIỆN CÓ
    # ----------------------------------------------------------------------------------
    add_h1("II. QUY TRÌNH NGHIỆP VỤ HIỆN CÓ (DƯỚI GÓC NHÌN THỰC TẾ)", bookmark="sec_2")
    
    add_h2("1. Nghiệp vụ 1: Quản lý thu – chi & Lập ngân sách cá nhân", bookmark="sec_2_1")
    add_bullet(" Ghi chép sổ tay, dùng Excel, ghi chú điện thoại hoặc chỉ ước lượng trong đầu.", bold_prefix="Hiện trạng thực hiện:", italic_prefix=True)
    add_bullet(" Người dùng chia tiền thành những \"ngăn vô hình\" (thiết yếu, hưởng thụ, tiết kiệm). Khi một ngăn bị âm, tâm lý buông xuôi dễ diễn ra khiến việc kiểm soát tài chính thất bại.", bold_prefix="Kế toán tâm lý (Mental Accounting):", italic_prefix=True)
    add_bullet(" Các khoản trừ tiền tự động hàng tháng (Netflix, Spotify, Cloud, tập Gym...) thường xuyên bị lãng quên.", bold_prefix="Bẫy chi tiêu định kỳ (Subscription Traps):", italic_prefix=True)
    add_bullet(" Mất nhiều giây và nhiều thao tác để ghi 1 giao dịch. Nếu quy trình vượt quá 3 bước, tỷ lệ người dùng bỏ cuộc sau 1 tuần rất cao.", bold_prefix="Tần suất & Điểm ma sát (Friction Score):", italic_prefix=True)

    add_h2("2. Nghiệp vụ 2: Tạo khoản chi chung & Đa dạng hóa cơ chế chia tiền (Split Bill)", bookmark="sec_2_2")
    add_bullet(" Vợ chồng đóng góp theo tỷ lệ 60–40, chi trả sinh hoạt phí và cấp tiền tiêu vặt (Allowance Quota) cho con cái.", bold_prefix="Mô hình Quỹ chung gia đình (Joint Pool):", italic_prefix=True)
    add_bullet(" Tiền phòng cố định tính riêng từng người kết hợp với tiền điện, nước, dịch vụ chia đều.", bold_prefix="Mô hình Sinh hoạt phòng trọ (Co-living):", italic_prefix=True)
    add_bullet(" Cá nhân bỏ tiền túi chi hộ việc chung của CLB/dự án → nộp hóa đơn về quỹ để nhận tiền hoàn ứng.", bold_prefix="Mô hình Tạm ứng & Hoàn ứng (Advance & Reimbursement):", italic_prefix=True)
    add_bullet(" Chia theo món ăn thực tế tiêu dùng (Itemized Split), chia theo tỷ lệ % hoặc chia theo suất/phần (người lớn 2 phần, trẻ em 1 phần).", bold_prefix="Mô hình Ăn uống & Du lịch:", italic_prefix=True)

    add_h2("3. Nghiệp vụ 3: Cấn trừ & Rút gọn chuỗi nợ nhóm", bookmark="sec_2_3")
    add_bullet(" Khi nhiều người cùng chi hộ trong chuyến đi nhiều ngày (A trả bữa 1, B trả bữa 2, C mua vé xe...), việc tính toán công nợ thủ công rất phức tạp.", bold_prefix="Hiện trạng tính nợ:", italic_prefix=True)
    add_bullet(" Cả nhóm phải thực hiện nhiều giao dịch chuyển khoản lòng vòng gây tốn thời gian, phát sinh phí và dễ nhầm lẫn.", bold_prefix="Điểm nghẽn giao dịch:", italic_prefix=True)
    add_bullet(" Áp dụng nguyên tắc bù trừ công nợ đa phương (Multilateral Netting) trên đồ thị nợ (Debt Simplification Graph) để rút gọn thành tối đa N - 1 giao dịch thanh toán trực tiếp.", bold_prefix="Cơ chế Số dư ròng (Net Balance Settlement):", italic_prefix=True)

    add_h2("4. Nghiệp vụ 4: Quyết toán nợ (Settle Up) & Xác thực minh chứng chuyển khoản", bookmark="sec_2_4")
    add_bullet(" Gõ tay số tài khoản ngân hàng → Rủi ro gõ nhầm số tiền hoặc nhầm người → Chụp màn hình bill gửi vào nhóm chat → Tin nhắn bị trôi và gây tranh cãi.", bold_prefix="Hiện trạng thực hiện:", italic_prefix=True)
    add_bullet(" Tự động sinh mã VietQR động chứa sẵn chính xác số tiền nợ và nội dung thanh toán, tự động kiểm tra chứng từ và cập nhật trạng thái nợ \"Đã thanh toán\".", bold_prefix="Nhu cầu chuẩn hóa:", italic_prefix=True)

    add_h2("5. Khảo sát các luồng ngoại lệ", bookmark="sec_2_5")
    add_bullet(" Quỹ chung nhóm bị âm hoặc hết tiền khi phát sinh chi phí đột xuất.", bold_prefix="Ngoại lệ 1 (Quỹ âm):", italic_prefix=True)
    add_bullet(" Ảnh chụp hóa đơn bị mờ, rách khiến AI OCR đọc sai số tiền (yêu cầu cơ chế Human-in-the-loop để người dùng chỉnh sửa nhanh).", bold_prefix="Ngoại lệ 2 (Lỗi nhận diện OCR):", italic_prefix=True)
    add_bullet(" Thành viên chuyển khoản sai số tiền hoặc chuyển nhầm vào tài khoản cá nhân thay vì quỹ nhóm.", bold_prefix="Ngoại lệ 3 (Sai sót chuyển khoản):", italic_prefix=True)

    # ----------------------------------------------------------------------------------
    # MỤC III. ĐÁNH GIÁ CÁC NGHIỆP VỤ HIỆN TẠI
    # ----------------------------------------------------------------------------------
    add_h1("III. ĐÁNH GIÁ CÁC NGHIỆP VỤ HIỆN TẠI", bookmark="sec_3")
    
    add_h2("1. Các khó khăn (Pain Points)", bookmark="sec_3_1")
    add_bullet(" Nhập liệu thủ công tốn thời gian khiến người dùng bỏ cuộc sau vài ngày.", bold_prefix="Rào cản ghi chép:", italic_prefix=True)
    add_bullet(" Áp dụng công thức chia đều cào bằng (Total / N) gây thiệt thòi cho người ăn ít hoặc về sớm.", bold_prefix="Chia tiền thiếu công bằng:", italic_prefix=True)
    add_bullet(" Phải chuyển khoản qua lại nhiều lần, tốn phí giao dịch và dễ nhầm lẫn số liệu.", bold_prefix="Mạng lưới nợ chéo phức tạp:", italic_prefix=True)
    add_bullet(" Ngại mở lời nhắc nợ bạn bè, sợ bị coi là \"tính toán chi li\".", bold_prefix="Rào cản tâm lý khi đòi nợ:", italic_prefix=True)
    add_bullet(" Cảnh báo tài chính truyền thống mang tính phán xét tiêu cực (\"Bạn đã tiêu vượt mức!\") thay vì đưa ra Cú hích hành động vi mô (Actionable Nudge) theo thời gian thực.", bold_prefix="Vỡ ngân sách cuối tháng:", italic_prefix=True)

    add_h2("2. Nguyên nhân cốt lõi", bookmark="sec_3_2")
    add_p("Dữ liệu tài chính phân mảnh trên nhiều nền tảng; thiếu công cụ tự động hóa bóc tách dữ liệu từ chứng từ hóa đơn; thiếu thuật toán tối ưu hóa đồ thị nợ chéo; việc giao tiếp công nợ gắn liền với cảm xúc cá nhân gây trở ngại tâm lý.")

    add_h2("3. Giải pháp nghiệp vụ (Độc lập công nghệ)", bookmark="sec_3_3")
    add_p("Ban hành bộ 4 quy chế chia tiền chuẩn mực (Equal, Exact, Percentage, Share); quy chuẩn hóa nguyên tắc bù trừ công nợ đa phương (Multilateral Netting); quy ước thời hạn chốt nợ trong 24–48 giờ sau sự kiện; khách quan hóa việc nhắc nợ thành các thông báo tự động mang tính hệ thống trung lập.")

    # ----------------------------------------------------------------------------------
    # MỤC IV. BÁO CÁO KHẢO SÁT HIỆN TRẠNG & ĐÁNH GIÁ ĐA GÓC NHÌN NGƯỜI DÙNG
    # ----------------------------------------------------------------------------------
    add_h1("IV. BÁO CÁO KHẢO SÁT HIỆN TRẠNG & ĐÁNH GIÁ ĐA GÓC NHÌN NGƯỜI DÙNG (PERSONA-BASED EVALUATION)", bookmark="sec_4")
    
    add_h2("1. Mục đích và phương pháp khảo sát", bookmark="sec_4_1")
    add_p("Khảo sát này thu thập và phân tích phản hồi định tính từ 11 nhóm đối tượng người dùng điển hình (Persona) có bối cảnh chi tiêu khác nhau. Các persona được xây dựng dựa trên các mô hình chi tiêu nhóm và cá nhân đã xác định trong hệ thống (sinh hoạt phòng trọ, du lịch nhóm, quỹ hộ gia đình, CLB/dự án khởi nghiệp, chi tiêu cá nhân), đồng thời mở rộng thêm các nhóm người dùng có khả năng tác động sâu sắc đến kiến trúc hệ thống (người lớn tuổi ít rành công nghệ, freelancer thu nhập biến động, cặp đôi sống chung, sinh viên năm nhất, chủ trọ/bên liên quan gián tiếp, và người dùng có kinh nghiệm sử dụng ứng dụng quốc tế).")
    add_p("Mỗi persona được phân tích và đánh giá toàn diện theo 4 khía cạnh: (1) Bối cảnh đặc thù, (2) Hiện trạng & Điểm nghẽn (Pain Point), (3) Phản hồi đối với giải pháp đề xuất của hệ thống, và (4) Các đề xuất bổ sung.")

    add_h2("2. Danh sách và đánh giá chi tiết 11 Persona khảo sát", bookmark="sec_4_2")

    # Persona 1
    add_h3("Persona 1: Sinh viên năm 3 — Chi tiêu cá nhân", bookmark="persona_1")
    add_p("Individual User · Expense Context", bold_prefix="Phân loại ngữ cảnh: ", italic_prefix=True)
    add_bullet(" Sinh viên trọ học, nguồn thu nhập kết hợp giữa trợ cấp gia đình và làm thêm bán thời gian; từng dùng ứng dụng Money Lover trong 2 tuần nhưng đã từ bỏ do thao tác nhập liệu rườm rà.", bold_prefix="Bối cảnh: ")
    add_bullet(" Thao tác ghi chép mỗi giao dịch tốn 4–5 bước (mở app, chọn ví, chọn danh mục, gõ số tiền, lưu); phần lớn chi tiêu nhỏ lẻ (trà sữa, gửi xe) qua chuyển khoản MoMo/QR không có hóa đơn giấy để chụp; cảnh báo vượt ngân sách chỉ xuất hiện cuối tháng khi tiền đã cạn nên không còn tác dụng điều chỉnh hành vi.", bold_prefix="Hiện trạng & Pain Point: ")
    add_bullet(" Đánh giá cao AI OCR giúp giảm điểm ma sát khi ghi chép; ủng hộ cơ chế phân bổ \"ngăn vô hình\" (Mental Accounting) và cảnh báo Nudge theo thời gian thực.", bold_prefix="Phản hồi giải pháp: ")
    add_bullet(" Bổ sung khả năng bóc tách dữ liệu từ ảnh chụp màn hình giao dịch chuyển khoản/ví điện tử; thêm chế độ ghi nhanh 1 chạm cho các khoản chi lặp lại; gửi cảnh báo Nudge theo ngưỡng thời gian thực (ví dụ: đã tiêu 80% ngân sách tuần).", bold_prefix="Đề xuất bổ sung: ")

    # Persona 2
    add_h3("Persona 2: Nhóm sinh viên ở ghép phòng trọ (4 người)", bookmark="persona_2")
    add_p("Group Member / Group Admin · Co-living Split · Group Settlement Context", bold_prefix="Phân loại ngữ cảnh: ", italic_prefix=True)
    add_bullet(" 4 sinh viên ở ghép chung phòng trọ; duy trì quỹ chung chi trả tiền điện, nước, internet và dịch vụ sinh hoạt; tiền phòng tính riêng cho từng cá nhân.", bold_prefix="Bối cảnh: ")
    add_bullet(" Bảng tính Excel tính được số tiền nhưng không giải quyết được việc nhắc nợ tế nhị; chia đều số lẻ phát sinh sai số làm tròn gây tranh cãi; thành viên đứng ra nộp tiền điện trước phải gánh rủi ro khi bạn cùng phòng chậm thanh toán.", bold_prefix="Hiện trạng & Pain Point: ")
    add_bullet(" Ủng hộ tính năng thông báo đòi nợ tự động giúp xóa bỏ rào cản tâm lý e ngại; giải pháp VietQR động chứa sẵn số tiền chính xác giúp triệt tiêu sai sót khi chuyển khoản tiền lẻ.", bold_prefix="Phản hồi giải pháp: ")
    add_bullet(" Cho phép tùy chỉnh giọng điệu/tần suất thông báo nhắc nợ (nhẹ nhàng – trung lập – nghiêm khắc); áp dụng quy tắc làm tròn số dư minh bạch; lưu trữ lịch sử thanh toán tiền dịch vụ theo từng kỳ.", bold_prefix="Đề xuất bổ sung: ")

    # Persona 3
    add_h3("Persona 3: Trưởng nhóm chuyến du lịch ngắn ngày (5 người)", bookmark="persona_3")
    add_p("Group Admin · Itemized / Share Split · Debt Simplification Context", bold_prefix="Phân loại ngữ cảnh: ", italic_prefix=True)
    add_bullet(" Nhóm bạn tổ chức chuyến du lịch 3 ngày; nhiều thành viên luân phiên đứng ra chi trả khách sạn, xe cộ, ăn uống và vé tham quan.", bold_prefix="Bối cảnh: ")
    add_bullet(" Từng thử nghiệm Splitwise nhưng giao diện tiếng Anh gây khó khăn cho thành viên lớn tuổi; sau chuyến đi phải thực hiện chuyển khoản lòng vòng nhiều lượt kéo dài cả tuần mới quyết toán xong; việc chia tiền theo từng món (Itemized Split) có thao tác nhập liệu phức tạp.", bold_prefix="Hiện trạng & Pain Point: ")
    add_bullet(" Thuật toán rút gọn chuỗi nợ (Debt Simplification Graph) được nhìn nhận là giá trị cốt lõi vượt trội; cơ chế chia theo suất (Share Split) rất thực tế khi có người lớn và trẻ em; giao diện tiếng Việt thân thiện.", bold_prefix="Phản hồi giải pháp: ")
    add_bullet(" Cho phép chỉnh sửa nhanh các khoản chi đã nhập sai mà không cần tạo lại; quy định mốc thời gian chốt nợ cụ thể (deadline 24–48 giờ sau chuyến đi) kèm nút xác nhận hoàn tất quyết toán; cung cấp chế độ hiển thị đơn giản hóa.", bold_prefix="Đề xuất bổ sung: ")

    # Persona 4
    add_h3("Persona 4: Phụ huynh quản lý quỹ chung gia đình", bookmark="persona_4")
    add_p("Joint Pool · Allowance Quota · Identity & Access Context", bold_prefix="Phân loại ngữ cảnh: ", italic_prefix=True)
    add_bullet(" Vợ chồng cùng đóng góp quỹ chung theo tỷ lệ 60–40 để chi trả sinh hoạt phí gia đình và cấp tiền tiêu vặt cho hai con.", bold_prefix="Bối cảnh: ")
    add_bullet(" Phương pháp ghi sổ tay tốn nhiều thời gian đối soát cuối tháng; muốn phân quyền cho con theo dõi chi tiêu nhằm giáo dục tài chính nhưng không cho phép chỉnh sửa dữ liệu; một số khoản chi cá nhân riêng tư không muốn hiển thị công khai trong báo cáo chung của gia đình.", bold_prefix="Hiện trạng & Pain Point: ")
    add_bullet(" Vai trò Người quan sát (Viewer / Auditor) cho con cái đáp ứng chính xác nhu cầu; cơ chế hạn mức tiêu dùng mỗi ngày (Daily Allowance) hỗ trợ theo dõi tiền tiêu vặt của con rất hiệu quả.", bold_prefix="Phản hồi giải pháp: ")
    add_bullet(" Bổ sung thuộc tính giao dịch riêng tư (Private Transaction) trong không gian nhóm; tách riêng báo cáo tài chính của con cái khỏi báo cáo sinh hoạt của cha mẹ; thiết lập cảnh báo tốc độ chi tiêu (Spending Velocity Alert) riêng cho quỹ tiêu vặt.", bold_prefix="Đề xuất bổ sung: ")

    # Persona 5
    add_h3("Persona 5: Thủ quỹ Câu lạc bộ / Dự án sinh viên", bookmark="persona_5")
    add_p("Treasurer / Primary Payer · Advance & Reimbursement Context", bold_prefix="Phân loại ngữ cảnh: ", italic_prefix=True)
    add_bullet(" Quản lý quỹ hoạt động cho tổ chức quy mô 30 thành viên; các thành viên thường xuyên tự ứng tiền túi mua vật phẩm sự kiện rồi nộp chứng từ đề nghị hoàn ứng.", bold_prefix="Bối cảnh: ")
    add_bullet(" Chứng từ hoàn ứng gửi qua tin nhắn chat bị trôi và dễ thất lạc; hóa đơn in nhiệt nhàu nát làm phát sinh sai sót khi nhập thủ công; thiếu nhật ký theo dõi ai đã phê duyệt khoản hoàn ứng nào.", bold_prefix="Hiện trạng & Pain Point: ")
    add_bullet(" Quy trình Tạm ứng → Nộp chứng từ → Hoàn ứng phản ánh đúng thực tế nghiệp vụ; bắt buộc phải có cơ chế Human-in-the-loop để thủ quỹ kiểm tra và xác nhận lại kết quả bóc tách từ AI OCR trước khi ghi nhận.", bold_prefix="Phản hồi giải pháp: ")
    add_bullet(" Bổ sung nhật ký kiểm toán (Audit Trail) lưu vết người duyệt và thời điểm phê duyệt; cho phép gắn thẻ (tag) phân loại chi phí theo từng sự kiện/dự án; xuất báo cáo tổng kết quỹ định kỳ dạng PDF/Excel phục vụ công khai tài chính.", bold_prefix="Đề xuất bổ sung: ")

    # Persona 6
    add_h3("Persona 6: Người lớn tuổi nghỉ hưu (Kỹ năng số cơ bản)", bookmark="persona_6")
    add_p("Individual User (Low Digital Literacy) · Budgeting Context", bold_prefix="Phân loại ngữ cảnh: ", italic_prefix=True)
    add_bullet(" Người lớn tuổi đã nghỉ hưu, quen ghi chép sổ tay truyền thống trong nhiều năm, được người thân hướng dẫn sử dụng ứng dụng điện thoại.", bold_prefix="Bối cảnh: ")
    add_bullet(" Thao tác gõ phím trên màn hình cảm ứng chậm; cỡ chữ và biểu tượng trên các ứng dụng thông thường quá nhỏ; việc phân chia cây danh mục chi tiêu phức tạp gây bối rối.", bold_prefix="Hiện trạng & Pain Point: ")
    add_bullet(" Khả năng chụp ảnh hóa đơn để AI tự động nhận diện số tiền giúp giảm đáng kể gánh nặng nhập liệu; yêu cầu hệ thống phải có giao diện thuần Việt, phông chữ lớn và tránh các thuật ngữ tài chính rườm rà.", bold_prefix="Phản hồi giải pháp: ")
    add_bullet(" Cung cấp chế độ hiển thị trợ năng (Accessibility Mode) với kích thước chữ lớn và giao diện tối giản; hỗ trợ nhập liệu bằng giọng nói (Voice Input); bổ sung vai trò người thân hỗ trợ quản lý từ xa mà không trực tiếp can thiệp dữ liệu.", bold_prefix="Đề xuất bổ sung: ")

    # Persona 7
    add_h3("Persona 7: Người làm nghề tự do (Freelancer) có thu nhập không cố định", bookmark="persona_7")
    add_p("Individual User · Budgeting Context · Spending Velocity Context", bold_prefix="Phân loại ngữ cảnh: ", italic_prefix=True)
    add_bullet(" Chuyên viên thiết kế tự do, nguồn thu nhập phát sinh linh hoạt theo dự án từng tháng, không có mức lương cố định.", bold_prefix="Bối cảnh: ")
    add_bullet(" Việc lập ngân sách theo chu kỳ cố định theo tháng không phù hợp với biến động dòng tiền thực tế; khó xác định mức chi tiêu an toàn mỗi ngày (Daily Allowance); mất nhiều thời gian tái thiết lập ngân sách thủ công mỗi khi có dự án mới.", bold_prefix="Hiện trạng & Pain Point: ")
    add_bullet(" Ngữ cảnh ngân sách cần hỗ trợ cơ chế linh hoạt theo dòng tiền thực tế; cảnh báo tốc độ tiêu tiền (Spending Velocity Alert) cần được tính toán dựa trên mức thu nhập trung bình 3 tháng gần nhất thay vì một con số cố định.", bold_prefix="Phản hồi giải pháp: ")
    add_bullet(" Cho phép thiết lập ngân sách động dựa trên trung bình động (Rolling Average); bổ sung danh mục chi phí vận hành công việc chuyên môn tách biệt khỏi chi tiêu sinh hoạt cá nhân; cảnh báo tốc độ tiêu tiền tự động điều chỉnh theo dòng tiền thực tế.", bold_prefix="Đề xuất bổ sung: ")

    # Persona 8
    add_h3("Persona 8: Cặp đôi sống chung (Chưa kết hôn)", bookmark="persona_8")
    add_p("2-Person Informal Split · Group Settlement Context", bold_prefix="Phân loại ngữ cảnh: ", italic_prefix=True)
    add_bullet(" Hai cá nhân sống chung nhưng chưa ràng buộc pháp lý; cùng chia sẻ tiền thuê nhà cố định và luân phiên chi trả các chi phí hẹn hò, sinh hoạt thường nhật.", bold_prefix="Bối cảnh: ")
    add_bullet(" Không phù hợp để thiết lập một mô hình quỹ gia đình pháp lý chính thức; tiền thuê nhà và tiền hẹn hò cần được quản lý riêng biệt; tâm lý e ngại khi sử dụng từ ngữ \"nợ\" (Debt) vì gây cảm giác thiếu tế nhị trong mối quan hệ tình cảm.", bold_prefix="Hiện trạng & Pain Point: ")
    add_bullet(" Đánh giá cao khả năng tạo nhiều không gian nhóm chi tiêu (ExpenseGroup) song song cho từng mục đích riêng biệt (quỹ tiền nhà, quỹ hẹn hò); đề xuất thay đổi nhãn hiển thị mang tính trung tính hơn (ví dụ: \"Cân bằng đóng góp\" thay vì \"Nợ\").", bold_prefix="Phản hồi giải pháp: ")
    add_bullet(" Cho phép cấu hình nhãn hiển thị công nợ linh hoạt tùy theo tính chất mối quan hệ; tối giản hóa quy trình quản trị nhóm cho các nhóm quy mô nhỏ (2 người) mà không cần phân quyền phức tạp.", bold_prefix="Đề xuất bổ sung: ")

    # Persona 9
    add_h3("Persona 9: Sinh viên năm nhất nhận trợ cấp cố định từ gia đình", bookmark="persona_9")
    add_p("Individual User · Behavioral Finance · Nudge Context", bold_prefix="Phân loại ngữ cảnh: ", italic_prefix=True)
    add_bullet(" Sinh viên mới bắt đầu cuộc sống tự lập xa nhà, nhận một khoản trợ cấp cố định hàng tháng từ phụ huynh, chưa có kinh nghiệm quản lý tài chính cá nhân.", bold_prefix="Bối cảnh: ")
    add_bullet(" Thường chi tiêu quá mức trong 2 tuần đầu tháng và rơi vào tình trạng thiếu hụt vào cuối tháng; chưa nắm được phương pháp phân bổ thu nhập vào các mục tiêu thiết yếu, phát triển và dự phòng; dễ bỏ quên các khoản thanh toán dịch vụ định kỳ (Subscription).", bold_prefix="Hiện trạng & Pain Point: ")
    add_bullet(" Rất cần các gợi ý phân bổ ngân sách mang tính hướng dẫn hành vi (Cú hích - Nudge) thay vì các cảnh báo thụ động khi ngân sách đã bị bội chi; hoan nghênh tính năng cảnh báo các bẫy đăng ký định kỳ (Subscription Traps).", bold_prefix="Phản hồi giải pháp: ")
    add_bullet(" Cung cấp sẵn các bộ mẫu ngân sách chuẩn (Budget Templates) dành cho người mới bắt đầu; tích hợp cơ chế khích lệ thói quen ghi chép (Gamification nhẹ); gửi thông báo nhắc nhở trước 3–5 ngày đối với các gói dịch vụ định kỳ sắp gia hạn.", bold_prefix="Đề xuất bổ sung: ")

    # Persona 10
    add_h3("Persona 10: Chủ nhà trọ quản lý thu tiền phòng từ nhiều phòng", bookmark="persona_10")
    add_p("Bên liên quan gián tiếp (Adjacent Stakeholder) · Co-living Context", bold_prefix="Phân loại ngữ cảnh: ", italic_prefix=True)
    add_bullet(" Quản lý khu trọ quy mô 10 phòng, định kỳ thu tiền thuê phòng và các khoản chi phí điện, nước, dịch vụ sinh hoạt từ đại diện từng phòng.", bold_prefix="Bối cảnh: ")
    add_bullet(" Mỗi phòng tự tổ chức chia tiền nội bộ, chủ nhà chỉ nhận tiền tổng và gặp khó khăn trong việc xác minh đại diện phòng đã thu đủ tiền từ tất cả thành viên hay chưa trước khi quyết toán.", bold_prefix="Hiện trạng & Pain Point: ")
    add_bullet(" Quan tâm đến việc mã VietQR động có thể hỗ trợ đại diện phòng thanh toán trực tiếp cho chủ nhà khi toàn bộ thành viên trong phòng đã hoàn tất nghĩa vụ đóng góp.", bold_prefix="Phản hồi giải pháp: ")
    add_bullet(" Ghi nhận khái niệm \"Người nhận tiền ngoài hệ thống\" (External Payee) cho giai đoạn phát triển mở rộng; hiển thị trạng thái tổng hợp nội bộ \"Đã thu đủ 100% thành viên\" trước khi kích hoạt giao dịch thanh toán ra bên ngoài.", bold_prefix="Đề xuất bổ sung: ")

    # Persona 11
    add_h3("Persona 11: Người dùng kinh nghiệm (Power-user) đã sử dụng Splitwise", bookmark="persona_11")
    add_p("Đối thủ cạnh tranh · Đánh giá chuẩn đối sánh (Benchmarking)", bold_prefix="Phân loại ngữ cảnh: ", italic_prefix=True)
    add_bullet(" Đã sử dụng thành thạo Splitwise trong hơn 2 năm cho nhiều nhóm bạn bè và các hoạt động du lịch, sự kiện.", bold_prefix="Bối cảnh: ")
    add_bullet(" Splitwise có ưu thế về các quy tắc chia tiền linh hoạt nhưng chưa tích hợp cổng thanh toán nội địa (VietQR/NAPAS) và thiếu tính năng AI OCR tự động nhận diện hóa đơn; giao diện tiếng Anh gây khó khăn cho người dùng phổ thông tại Việt Nam.", bold_prefix="Hiện trạng & Pain Point: ")
    add_bullet(" Điểm khác biệt cạnh tranh lớn nhất là việc tích hợp đồng bộ mã VietQR động và AI OCR ngay trong luồng nghiệp vụ chia tiền; hệ thống 4 quy tắc chia tiền (Equal, Exact, Percentage, Share) hoàn toàn đáp ứng các tiêu chuẩn nghiệp vụ quốc tế.", bold_prefix="Phản hồi giải pháp: ")
    add_bullet(" Nhấn mạnh lợi thế bản địa hóa sâu sắc (tích hợp VietQR, giao diện tiếng Việt chuẩn hóa, phù hợp thói quen tiêu dùng trong nước); cung cấp tính năng xuất và nhập dữ liệu lịch sử giao dịch nhằm hỗ trợ người dùng chuyển đổi nền tảng dễ dàng.", bold_prefix="Đề xuất bổ sung: ")

    # ----------------------------------------------------------------------------------
    # BẢNG TỔNG HỢP PAIN POINT THEO BOUNDED CONTEXT
    # ----------------------------------------------------------------------------------
    add_h2("3. Tổng hợp Pain Point theo Bounded Context [Bảng 1]", bookmark="sec_4_3")
    add_p("Bảng dưới đây tổng hợp và đối chiếu các Pain Point ghi nhận từ 11 Persona khảo sát theo 4 Bounded Context cốt lõi (và nhóm mở rộng ngoài phạm vi), làm cơ sở trực tiếp cho việc thiết kế mô hình miền (Domain-Driven Design). Nhấp vào số hiệu Persona để chuyển nhanh đến phần phân tích chi tiết.")
    
    tbl_bp = doc.add_table(rows=6, cols=4)
    tbl_bp_widths = [Inches(1.5), Inches(2.2), Inches(0.8), Inches(2.0)]
    bp_headers = ["Bounded Context", "Pain Point Tổng Hợp", "Persona", "Đề Xuất Giải Pháp Hệ Thống"]
    bp_rows = [
        ["Expense Context", "Ghi chép thủ công tốn thao tác; bỏ sót giao dịch qua chuyển khoản/QR (không phải hóa đơn giấy).", "1, 6, 9", "AI OCR mở rộng đọc ảnh chụp màn hình giao dịch ngân hàng/ví điện tử; ghi nhanh 1 chạm; voice input."],
        ["Group Settlement Context", "Đòi nợ gây ngại về mặt tâm lý; chia đều cào bằng gây thiệt thòi; chuyển khoản lòng vòng kéo dài.", "2, 3, 8", "Thông báo đòi nợ tự động tùy chỉnh giọng điệu; thuật toán Debt Simplification; hỗ trợ nhiều ExpenseGroup song song."],
        ["Budgeting Context", "Ngân sách chu kỳ cố định không phù hợp thu nhập biến động; cảnh báo đến quá trễ (cuối tháng).", "1, 7, 9", "Ngân sách động theo rolling average; Nudge thời gian thực; cung cấp mẫu ngân sách chuẩn cho người mới."],
        ["Identity & Access Context", "Thiếu phân quyền riêng tư trong nhóm gia đình; thiếu vai trò hỗ trợ từ xa cho người lớn tuổi.", "4, 6", "Thuộc tính giao dịch riêng tư/ẩn; vai trò \"người thân hỗ trợ\" tách biệt khỏi Viewer thông thường."],
        ["Ngoài phạm vi 4 Context (Mở rộng)", "Thiếu audit trail cho duyệt hoàn ứng; chưa có vai trò cho bên nhận tiền ngoài nhóm (chủ trọ).", "5, 10", "Bổ sung Audit Trail cho Reimbursement; ghi nhận khái niệm \"Payee bên ngoài\" ở giai đoạn mở rộng."]
    ]
    style_table(tbl_bp, tbl_bp_widths, bp_headers, bp_rows, is_persona_col=2)
    doc.add_paragraph().paragraph_format.space_after = Pt(6)

    # ----------------------------------------------------------------------------------
    # BẢNG PHÂN LOẠI MOSCOW
    # ----------------------------------------------------------------------------------
    add_h2("4. Ưu tiên hóa đề xuất bổ sung theo mô hình MoSCoW [Bảng 2]", bookmark="sec_4_4")
    add_p("Các đề xuất tính năng từ khảo sát người dùng được phân loại theo ma trận ưu tiên MoSCoW nhằm định hình phạm vi phân tích Use Case và thiết kế kiến trúc trong các giai đoạn tiếp theo. Nhấp vào số hiệu Persona để xem cơ sở đề xuất.")
    
    tbl_moscow = doc.add_table(rows=9, cols=4)
    tbl_moscow_widths = [Inches(1.3), Inches(2.1), Inches(2.2), Inches(0.9)]
    moscow_headers = ["Mức Ưu Tiên", "Đề Xuất Tính Năng", "Lý Do Nghiệp Vụ & Thực Tiễn", "Persona"]
    moscow_rows = [
        ["Must (Bắt buộc)", "Human-in-the-loop chỉnh sửa nhanh khi AI OCR đọc sai", "Ảnh hóa đơn mờ/rách rất phổ biến trong thực tế; không thể để AI tự quyết số tiền quyết toán cuối cùng.", "5, 6"],
        ["Must (Bắt buộc)", "VietQR động chứa sẵn số tiền chính xác", "Giải quyết triệt để lỗi chuyển khoản nhầm số tiền/số lẻ — vấn đề lặp lại ở hầu hết các nhóm khảo sát.", "2, 3, 10"],
        ["Should (Nên có)", "Nudge thời gian thực thay vì cảnh báo cuối kỳ", "Cảnh báo trễ không còn tác dụng điều chỉnh hành vi chi tiêu; Nudge tức thời mang lại giá trị cao.", "1, 9"],
        ["Should (Nên có)", "Audit Trail cho quy trình duyệt hoàn ứng", "Đảm bảo tính minh bạch tài chính tập thể bắt buộc đối với các tổ chức CLB, dự án sinh viên.", "5"],
        ["Should (Nên có)", "Ngân sách động theo thu nhập trung bình (Rolling)", "Đáp ứng chính xác nhu cầu của nhóm người làm nghề tự do (Freelancer) và người có thu nhập biến động.", "7"],
        ["Could (Có thể)", "Chế độ giao diện trợ năng (Accessibility Mode) / Voice Input", "Tối ưu hóa khả năng tiếp cận hệ thống cho nhóm người dùng lớn tuổi, ít rành công nghệ.", "6"],
        ["Could (Có thể)", "Nhiều ExpenseGroup song song cho nhóm nhỏ (Cặp đôi)", "Phù hợp với các mối quan hệ linh hoạt không muốn gộp chung một quỹ gia đình cứng nhắc.", "8"],
        ["Won't (Giai đoạn sau)", "Hỗ trợ đa tiền tệ (Multi-currency); Payee bên ngoài hệ thống", "Vượt quá phạm vi 4 Bounded Context cốt lõi của Lab 1; ghi nhận cho vòng khảo sát mở rộng.", "10, 11"]
    ]
    style_table(tbl_moscow, tbl_moscow_widths, moscow_headers, moscow_rows, is_persona_col=3)
    doc.add_paragraph().paragraph_format.space_after = Pt(6)

    # ----------------------------------------------------------------------------------
    # BẢNG SO SÁNH ĐỐI THỦ CẠNH TRANH (BENCHMARKING)
    # ----------------------------------------------------------------------------------
    add_h2("5. So sánh đối thủ cạnh tranh (Benchmarking theo phản hồi Persona) [Bảng 3]", bookmark="sec_4_5")
    add_p("Tổng hợp đánh giá so sánh trực tiếp giữa các giải pháp quản lý tài chính hiện có trên thị trường dựa trên trải nghiệm thực tế của các nhóm người dùng khảo sát.")
    
    tbl_bench = doc.add_table(rows=5, cols=4)
    tbl_bench_widths = [Inches(1.4), Inches(2.2), Inches(2.1), Inches(0.8)]
    bench_headers = ["Ứng Dụng", "Điểm Mạnh (Theo Khảo Sát)", "Điểm Yếu (Theo Khảo Sát)", "Persona"]
    bench_rows = [
        ["Splitwise", "Linh hoạt 4 kiểu chia tiền, chuẩn quốc tế, hỗ trợ nhóm du lịch tốt.", "Không tích hợp thanh toán nội địa VietQR; giao diện tiếng Anh; không có AI OCR nhận diện hóa đơn.", "11"],
        ["Money Lover", "Phổ biến tại Việt Nam, quản lý danh mục chi tiêu đa dạng.", "Thao tác ghi chép nhiều bước gây nản; cảnh báo ngân sách thụ động, mang tính phán xét và đến trễ.", "1"],
        ["Sổ Thu Chi / Ghi sổ tay", "Đơn giản, trực quan, thân thuộc với người lớn tuổi.", "Không hỗ trợ tính toán công nợ nhóm; tốn nhiều thời gian đối soát; dễ thất lạc chứng từ.", "4, 6"],
        ["Spendee", "Giao diện hiện đại, công cụ lập ngân sách trực quan.", "Ngân sách cố định theo chu kỳ; phải tái thiết lập thủ công khi dòng tiền thu nhập thay đổi.", "7"]
    ]
    style_table(tbl_bench, tbl_bench_widths, bench_headers, bench_rows, is_persona_col=3)
    doc.add_paragraph().paragraph_format.space_after = Pt(6)

    # ----------------------------------------------------------------------------------
    # KẾT LUẬN KHẢO SÁT
    # ----------------------------------------------------------------------------------
    add_h2("6. Kết luận khảo sát và định hướng mở rộng", bookmark="sec_4_6")
    add_p("Kết quả khảo sát từ 11 nhóm đối tượng người dùng giả lập đã xác lập 3 định hướng thiết kế trọng tâm xuyên suốt cho toàn bộ hệ thống:")
    add_bullet(" Mọi nhóm người dùng cá nhân đều gặp trở ngại lớn với việc nhập liệu thủ công. Hệ thống cần tích hợp AI OCR đa phương thức có khả năng xử lý cả hóa đơn giấy in nhiệt lẫn ảnh chụp màn hình giao dịch chuyển khoản / ví điện tử, kết hợp tính năng ghi nhanh 1 chạm.", bold_prefix="Giảm thiểu tối đa ma sát nhập liệu: ")
    add_bullet(" Từ sinh viên ở ghép đến các cặp đôi hay ban quản trị CLB, rào cản tâm lý khi trực tiếp nhắc nợ hoặc phán xét chi tiêu là nguyên nhân chính dẫn đến tranh chấp. Hệ thống cần đóng vai trò trung gian khách quan thông qua các thông báo đòi nợ tự động và gợi ý Nudge mang tính hỗ trợ vi mô.", bold_prefix="Khách quan hóa việc nhắc nợ và cảnh báo ngân sách: ")
    add_bullet(" Hệ thống không áp đặt một mô hình quản lý cứng nhắc mà hỗ trợ đa dạng hóa không gian chi tiêu (ExpenseGroup) song song, cho phép tùy biến nhãn gọi danh xưng, cấu hình mức độ riêng tư của giao dịch và phân quyền linh hoạt theo từng cấu trúc quan hệ xã hội thực tế.", bold_prefix="Linh hoạt hóa theo bối cảnh quan hệ xã hội: ")
    add_p("Các persona mở rộng (người lớn tuổi, freelancer, cặp đôi, sinh viên năm nhất, chủ trọ, power-user Splitwise) cung cấp dữ liệu định tính quý giá để nhóm phát triển hoàn thiện tài liệu phân tích hiện trạng và định hướng chính xác cho các giai đoạn thiết kế Use Case, Class Diagram và Database Schema ở các bài Lab tiếp theo.")

    # ----------------------------------------------------------------------------------
    # MỤC V. CÁC TÁC NHÂN VÀ MÔ HÌNH THỰC THỂ NGHIỆP VỤ
    # ----------------------------------------------------------------------------------
    add_h1("V. CÁC TÁC NHÂN (ACTOR) VÀ MÔ HÌNH THỰC THỂ NGHIỆP VỤ", bookmark="sec_5")
    
    add_h2("1. Các tác nhân ngoài (External Actors)", bookmark="sec_5_1")
    add_bullet(" Quản lý ví tiền cá nhân, ghi nhận thu/chi, thiết lập ngân sách và nhận các gợi ý điều chỉnh chi tiêu (Nudges).", bold_prefix="Individual User (Người dùng cá nhân):", italic_prefix=True)
    add_bullet(" Khởi tạo nhóm, phân quyền thành viên, cấu hình quy tắc chia tiền, duyệt hoàn ứng và chốt quyết toán.", bold_prefix="Group Admin (Trưởng nhóm):", italic_prefix=True)
    add_bullet(" Tham gia nhóm, ghi nhận chi phí chung, thanh toán công nợ qua VietQR và nộp minh chứng chuyển khoản.", bold_prefix="Group Member (Thành viên nhóm):", italic_prefix=True)
    add_bullet(" Ứng tiền thanh toán trước, chụp ảnh hóa đơn, xác nhận đã nhận tiền hoàn trả.", bold_prefix="Treasurer / Primary Payer (Thủ quỹ / Người chi trả):", italic_prefix=True)
    add_bullet(" Theo dõi báo cáo thu chi và số dư quỹ nhóm mà không có quyền thay đổi dữ liệu.", bold_prefix="Auditor / Viewer (Người quan sát):", italic_prefix=True)
    add_bullet(" Chuẩn hóa cây danh mục toàn cục, giám sát an toàn dữ liệu và xử lý khiếu nại.", bold_prefix="System Administrator (Quản trị viên hệ thống):", italic_prefix=True)
    add_bullet(" Cung cấp API sinh mã QR động chuẩn NAPAS và webhook xác thực giao dịch chuyển khoản.", bold_prefix="VietQR / Banking Gateway Partner (Đối tác Ngân hàng):", italic_prefix=True)
    add_bullet(" Bóc tách dữ liệu từ ảnh chụp hóa đơn đa phương thức (Multimodal Vision).", bold_prefix="AI OCR Receipt Engine (Dịch vụ Trí tuệ nhân tạo):", italic_prefix=True)

    add_h2("2. Các đối tượng thực thể nghiệp vụ (Domain Entities & Value Objects)", bookmark="sec_5_2")
    add_bullet(" Quản lý thông tin định danh, tài khoản, cấu hình tiền tệ mặc định.", bold_prefix="UserAccount (Entity):", italic_prefix=True)
    add_bullet(" Quản lý nguồn tài chính (Tiền mặt, Ngân hàng, Ví điện tử) và số dư khả dụng (debit, credit, transferTo).", bold_prefix="Wallet (Entity):", italic_prefix=True)
    add_bullet(" Lưu trữ số tiền (Decimal chính xác cao) và đơn vị tiền tệ (Currency), triệt tiêu hoàn toàn sai số làm tròn khi chia tiền lẻ.", bold_prefix="Money (Value Object bất biến):", italic_prefix=True)
    add_bullet(" Cây phân loại mục đích chi tiêu (Ăn uống, Đi lại, Nhà ở, Chi thiết yếu vs Chi hưởng thụ).", bold_prefix="Category & SubCategory (Entity):", italic_prefix=True)
    add_bullet(" Hạn mức chi tiêu theo chu kỳ và danh mục, tính toán số tiền được phép tiêu mỗi ngày (Daily Allowance).", bold_prefix="Budget (Entity):", italic_prefix=True)
    add_bullet(" Lớp cơ sở trừu tượng với các phân lớp con đa hình kế thừa:", bold_prefix="Transaction (Abstract Base Class):", italic_prefix=True)
    add_bullet(" Khoản chi tiêu cá nhân thường nhật.", bold_prefix="   • PersonalExpense: ", italic_prefix=False)
    add_bullet(" Khoản thu nhập ghi nhận vào ví.", bold_prefix="   • IncomeTransaction: ", italic_prefix=False)
    add_bullet(" Khoản chi chung của nhóm, xác định Người trả tiền (Payer) và Danh sách người cùng chia tiền.", bold_prefix="   • GroupExpense: ", italic_prefix=False)
    add_bullet(" Đơn vị ranh giới quản lý không gian chi tiêu chung (quỹ chung, danh sách thành viên, quy chế chia tiền, trạng thái hoạt động).", bold_prefix="ExpenseGroup (Aggregate Root):", italic_prefix=True)
    add_bullet(" Lớp chiến lược (Strategy Pattern) với các lớp con đa hình thực thi quy tắc phân bổ:", bold_prefix="SplitRule (Strategy Pattern):", italic_prefix=True)
    add_bullet(" Chia đều cào bằng (Total / N).", bold_prefix="   • EqualSplitRule: ", italic_prefix=False)
    add_bullet(" Chia theo số tiền đích danh từng người.", bold_prefix="   • ExactSplitRule: ", italic_prefix=False)
    add_bullet(" Chia theo tỷ lệ % (Tổng tỷ lệ = 100%).", bold_prefix="   • PercentageSplitRule: ", italic_prefix=False)
    add_bullet(" Chia theo số phần/suất đóng góp (ví dụ: người lớn 2 phần, trẻ em 1 phần).", bold_prefix="   • ShareSplitRule: ", italic_prefix=False)
    add_bullet(" Ghi nhận số tiền và trách nhiệm chi trả của từng thành viên trong một khoản chi nhóm.", bold_prefix="SplitDetail (Entity):", italic_prefix=True)
    add_bullet(" Lưu trữ quan hệ công nợ có hướng giữa 2 cá nhân (Debtor → Creditor).", bold_prefix="DebtRelation (Entity):", italic_prefix=True)
    add_bullet(" Đồ thị xử lý cấn trừ nợ chéo nhiều bên và rút gọn chuỗi nợ dựa trên Số dư ròng (Net Balance - Thuật toán Robert Tarjan).", bold_prefix="DebtSimplificationGraph (Domain Service):", italic_prefix=True)
    add_bullet(" Giao dịch thanh toán tất toán nợ thực tế giữa 2 thành viên trong nhóm.", bold_prefix="Settlement (Entity):", italic_prefix=True)
    add_bullet(" Lưu trữ ảnh chụp chuyển khoản, mã tham chiếu ngân hàng (Bank Reference Code) và trạng thái đối soát.", bold_prefix="PaymentProof (Entity):", italic_prefix=True)
    add_bullet(" Theo dõi tốc độ tiêu tiền (Burn Rate) theo thời gian thực và phát cảnh báo nguy cơ vỡ ngân sách trước ngày 20 hàng tháng.", bold_prefix="SpendingVelocityAlert (Domain Service):", italic_prefix=True)
    add_bullet(" Dữ liệu bóc tách từ ảnh hóa đơn (tên điểm bán, ngày giờ, danh sách món, thuế VAT, tổng tiền).", bold_prefix="OCRReceipt (Entity):", italic_prefix=True)

    # ----------------------------------------------------------------------------------
    # MỤC VI. HIỆN TRẠNG TIN HỌC
    # ----------------------------------------------------------------------------------
    add_h1("VI. HIỆN TRẠNG TIN HỌC", bookmark="sec_6")
    
    add_h2("1. Phần cứng", bookmark="sec_6_1")
    add_bullet(" Smartphone chạy hệ điều hành Android hoặc iOS có camera tối thiểu 8MP để chụp rõ nét hóa đơn trong điều kiện thiếu sáng; Laptop/PC để xem báo cáo phân tích tài chính dạng bảng lớn và trực quan hóa đồ thị.", bold_prefix="Thiết bị người dùng:", italic_prefix=True)
    add_bullet(" Máy chủ ứng dụng (Application Server), Máy chủ CSDL (Database Server) và Lưu trữ đối tượng (Object Storage chuẩn S3-compatible) nhằm lưu trữ an toàn hình ảnh hóa đơn và chứng từ thanh toán.", bold_prefix="Hạ tầng máy chủ:", italic_prefix=True)

    add_h2("2. Phần mềm", bookmark="sec_6_2")
    add_bullet(" Excel, Google Sheets, App ghi chú, App Mobile Banking (Vietcombank, MB Bank, Techcombank...), Ví điện tử (MoMo, ZaloPay), Ứng dụng chat (Zalo, Messenger, Telegram).", bold_prefix="Phần mềm người dùng đang sử dụng:", italic_prefix=True)
    add_bullet(" iOS, Android, Web Application (Windows / macOS).", bold_prefix="Hệ điều hành mục tiêu:", italic_prefix=True)
    add_bullet(" PostgreSQL / MySQL (chuẩn hóa 3NF, hỗ trợ Transactional Locking và đảm bảo tính toàn vẹn ACID trong giao dịch tài chính).", bold_prefix="Hệ quản trị CSDL dự kiến:", italic_prefix=True)
    add_bullet(" Dịch vụ AI Vision OCR (Google Gemini / Vision API), Chuẩn sinh mã VietQR động của NAPAS.", bold_prefix="Dịch vụ / API tích hợp:", italic_prefix=True)

    add_h2("3. Con người", bookmark="sec_6_3")
    add_bullet(" Nhóm đối tượng người dùng mục tiêu (học sinh, sinh viên, người đi làm trẻ tuổi) cực kỳ thành thạo smartphone, quen thuộc với thanh toán không tiền mặt và quét mã QR. Người dùng kỳ vọng trải nghiệm frictionless 0–1 chạm và không chấp nhận các giao diện nhập liệu thủ công rườm rà.", bold_prefix="Trình độ Tin học & Kỳ vọng trải nghiệm:", italic_prefix=True)

    # Save document with fallback if locked by Word
    try:
        doc.save(output_path)
        print(f"Document successfully created and saved to: {output_path}")
    except PermissionError:
        fallback_path = output_path.replace(".docx", "_v2.docx")
        doc.save(fallback_path)
        print(f"Original file was open in Word. Saved to: {fallback_path}")

if __name__ == "__main__":
    create_full_report_with_native_headings("Lab1_BaoCao_KhaoSat_HienTrang_TongHop.docx")
