import docx
from docx.shared import Inches, Pt, Cm, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.enum.table import WD_TABLE_ALIGNMENT, WD_ALIGN_VERTICAL
from docx.oxml import OxmlElement, parse_xml
from docx.oxml.ns import nsdecls, qn
import os

def set_cell_shading(cell, color_hex):
    shading_xml = f'<w:shd {nsdecls("w")} w:fill="{color_hex}"/>'
    cell._tc.get_or_add_tcPr().append(parse_xml(shading_xml))

def set_cell_margins(cell, top=100, bottom=100, left=150, right=150):
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

def set_table_borders(table, color="CBD5E1", sz="4"):
    tblPr = table._tbl.tblPr
    borders_xml = f'''
        <w:tblBorders {nsdecls("w")}>
            <w:top w:val="single" w:sz="{sz}" w:space="0" w:color="{color}"/>
            <w:left w:val="single" w:sz="{sz}" w:space="0" w:color="{color}"/>
            <w:bottom w:val="single" w:sz="{sz}" w:space="0" w:color="{color}"/>
            <w:right w:val="single" w:sz="{sz}" w:space="0" w:color="{color}"/>
            <w:insideH w:val="single" w:sz="{sz}" w:space="0" w:color="{color}"/>
            <w:insideV w:val="single" w:sz="{sz}" w:space="0" w:color="{color}"/>
        </w:tblBorders>
    '''
    tblPr.append(parse_xml(borders_xml))

def add_bookmark(p, bookmark_id, bookmark_name):
    bm_start = parse_xml(f'<w:bookmarkStart {nsdecls("w")} w:id="{bookmark_id}" w:name="{bookmark_name}"/>')
    bm_end = parse_xml(f'<w:bookmarkEnd {nsdecls("w")} w:id="{bookmark_id}"/>')
    p._p.insert(0, bm_start)
    p._p.append(bm_end)

def set_p_outline(p, level):
    pPr = p._p.get_or_add_pPr()
    pPr.append(parse_xml(f'<w:outlineLvl {nsdecls("w")} w:val="{level}"/>'))

def add_toc_line(p, title, page_num, bookmark_name=None):
    p.paragraph_format.line_spacing = 1.3
    p.paragraph_format.space_before = Pt(3)
    p.paragraph_format.space_after = Pt(3)
    p.paragraph_format.first_line_indent = Cm(0)
    
    # Tab stop at 15.5 cm with dot leader
    pPr = p._p.get_or_add_pPr()
    tabs_xml = f'''
        <w:tabs {nsdecls("w")}>
            <w:tab w:val="right" w:leader="dot" w:pos="8790"/>
        </w:tabs>
    '''
    pPr.append(parse_xml(tabs_xml))
    
    safe_title = title.replace('&', '&amp;').replace('<', '&lt;').replace('>', '&gt;')
    if bookmark_name:
        hl_xml = f'''
            <w:hyperlink {nsdecls("w")} w:anchor="{bookmark_name}" w:history="1">
                <w:r>
                    <w:rPr>
                        <w:rFonts w:ascii="Times New Roman" w:hAnsi="Times New Roman"/>
                        <w:sz w:val="26"/>
                        <w:color w:val="000000"/>
                    </w:rPr>
                    <w:t>{safe_title}</w:t>
                </w:r>
                <w:r>
                    <w:rPr>
                        <w:rFonts w:ascii="Times New Roman" w:hAnsi="Times New Roman"/>
                        <w:sz w:val="26"/>
                        <w:color w:val="000000"/>
                    </w:rPr>
                    <w:tab/>
                </w:r>
                <w:r>
                    <w:rPr>
                        <w:rFonts w:ascii="Times New Roman" w:hAnsi="Times New Roman"/>
                        <w:sz w:val="26"/>
                        <w:color w:val="000000"/>
                    </w:rPr>
                    <w:t>{page_num}</w:t>
                </w:r>
            </w:hyperlink>
        '''
        p._p.append(parse_xml(hl_xml))
    else:
        r1 = p.add_run(title)
        r1.font.name = "Times New Roman"
        r1.font.size = Pt(13)
        r2 = p.add_run("\t" + str(page_num))
        r2.font.name = "Times New Roman"
        r2.font.size = Pt(13)

def add_dynamic_field(p, field_instr, default_text=None, is_bold=False, is_italic=False):
    escaped_instr = field_instr.replace('&', '&amp;').replace('"', '&quot;').replace('<', '&lt;').replace('>', '&gt;')
    if default_text:
        b_tag = '<w:b/>' if is_bold else ''
        i_tag = '<w:i/>' if is_italic else ''
        fld_xml = f'''
            <w:fldSimple {nsdecls("w")} w:instr="{escaped_instr}">
                <w:r>
                    <w:rPr>
                        <w:rFonts w:ascii="Times New Roman" w:hAnsi="Times New Roman"/>
                        <w:sz w:val="24"/>
                        {b_tag}
                        {i_tag}
                    </w:rPr>
                    <w:t>{default_text}</w:t>
                </w:r>
            </w:fldSimple>
        '''
        fld = parse_xml(fld_xml)
    else:
        fld = parse_xml(f'<w:fldSimple {nsdecls("w")} w:instr="{escaped_instr}"/>')
    p._p.append(fld)


def main():
    doc = docx.Document()
    
    # 1. Default Style Font
    style = doc.styles['Normal']
    font = style.font
    font.name = 'Times New Roman'
    font.size = Pt(13)
    font.color.rgb = RGBColor(0, 0, 0)
    
    # 2. Page Setup (A4 & DNTU Format: Top 2.0cm, Bottom 2.0cm, Left 3.5cm, Right 2.0cm)
    for section in doc.sections:
        section.page_width = Cm(21.0)
        section.page_height = Cm(29.7)
        section.top_margin = Cm(2.0)
        section.bottom_margin = Cm(2.0)
        section.left_margin = Cm(3.5)
        section.right_margin = Cm(2.0)
        
        # Cover page has no page number
        section.different_first_page_header_footer = True
        
        # Footer for pages 2+: centered page number
        footer = section.footer
        f_p = footer.paragraphs[0]
        f_p.alignment = WD_ALIGN_PARAGRAPH.CENTER
        f_run = f_p.add_run()
        f_run.font.name = 'Times New Roman'
        f_run.font.size = Pt(12)
        f_fld = parse_xml(r'<w:fldSimple %s w:instr="PAGE"/>' % nsdecls('w'))
        f_p._p.append(f_fld)
        
        # First page footer (empty)
        first_footer = section.first_page_footer
        first_footer.paragraphs[0].text = ""

    # =========================================================================
    # TRANG 1: BÌA (theo đúng yêu cầu: bộ giáo dục và đào tạo, logo, tên đề tài, nhóm sv, người hướng dẫn)
    # =========================================================================
    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p.paragraph_format.space_before = Pt(0)
    p.paragraph_format.space_after = Pt(2)
    p.paragraph_format.line_spacing = 1.2
    p.paragraph_format.first_line_indent = Cm(0)
    r = p.add_run("BỘ GIÁO DỤC VÀ ĐÀO TẠO")
    r.font.name = "Times New Roman"
    r.font.size = Pt(13)
    r.bold = True

    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p.paragraph_format.space_before = Pt(2)
    p.paragraph_format.space_after = Pt(4)
    p.paragraph_format.line_spacing = 1.2
    p.paragraph_format.first_line_indent = Cm(0)
    r = p.add_run("TRƯỜNG ĐẠI HỌC CÔNG NGHỆ ĐỒNG NAI")
    r.font.name = "Times New Roman"
    r.font.size = Pt(14)
    r.bold = True

    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p.paragraph_format.space_before = Pt(0)
    p.paragraph_format.space_after = Pt(14)
    p.paragraph_format.line_spacing = 1.2
    p.paragraph_format.first_line_indent = Cm(0)
    r = p.add_run("KHOA CÔNG NGHỆ THÔNG TIN\n———————— • ————————")
    r.font.name = "Times New Roman"
    r.font.size = Pt(12)
    r.bold = True

    # Logo trường
    logo_path = "scratch/image1.png"
    if os.path.exists(logo_path):
        p_logo = doc.add_paragraph()
        p_logo.alignment = WD_ALIGN_PARAGRAPH.CENTER
        p_logo.paragraph_format.space_before = Pt(8)
        p_logo.paragraph_format.space_after = Pt(16)
        p_logo.paragraph_format.first_line_indent = Cm(0)
        run_logo = p_logo.add_run()
        run_logo.add_picture(logo_path, width=Cm(3.2))

    # Tên bài báo cáo môn học
    p_sub = doc.add_paragraph()
    p_sub.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p_sub.paragraph_format.space_before = Pt(12)
    p_sub.paragraph_format.space_after = Pt(6)
    p_sub.paragraph_format.line_spacing = 1.3
    p_sub.paragraph_format.first_line_indent = Cm(0)
    r = p_sub.add_run("BÁO CÁO ĐỒ ÁN MÔN HỌC\nLẬP TRÌNH THIẾT BỊ DI ĐỘNG")
    r.font.name = "Times New Roman"
    r.font.size = Pt(15)
    r.bold = True

    # Tên đề tài
    p_title = doc.add_paragraph()
    p_title.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p_title.paragraph_format.space_before = Pt(8)
    p_title.paragraph_format.space_after = Pt(24)
    p_title.paragraph_format.line_spacing = 1.3
    p_title.paragraph_format.first_line_indent = Cm(0)
    r = p_title.add_run("TÊN ĐỀ TÀI:\nỨNG DỤNG QUẢN LÝ CHI TIÊU CÁ NHÂN\n(PERSONAL EXPENSE TRACKER)")
    r.font.name = "Times New Roman"
    r.font.size = Pt(16)
    r.bold = True
    r.font.color.rgb = RGBColor(15, 44, 89)

    # Nhóm SV & Người hướng dẫn
    p_info = doc.add_paragraph()
    p_info.alignment = WD_ALIGN_PARAGRAPH.LEFT
    p_info.paragraph_format.left_indent = Cm(2.0)
    p_info.paragraph_format.first_line_indent = Cm(0)
    p_info.paragraph_format.space_before = Pt(12)
    p_info.paragraph_format.space_after = Pt(4)
    p_info.paragraph_format.line_spacing = 1.3
    r = p_info.add_run("Người hướng dẫn: ThS. Nguyễn Thị Liệu")
    r.font.name = "Times New Roman"
    r.font.size = Pt(13)
    r.bold = True

    p_info2 = doc.add_paragraph()
    p_info2.alignment = WD_ALIGN_PARAGRAPH.LEFT
    p_info2.paragraph_format.left_indent = Cm(2.0)
    p_info2.paragraph_format.first_line_indent = Cm(0)
    p_info2.paragraph_format.space_before = Pt(4)
    p_info2.paragraph_format.space_after = Pt(4)
    p_info2.paragraph_format.line_spacing = 1.3
    r = p_info2.add_run("Nhóm sinh viên thực hiện:")
    r.font.name = "Times New Roman"
    r.font.size = Pt(13)
    r.bold = True

    p_info3 = doc.add_paragraph()
    p_info3.alignment = WD_ALIGN_PARAGRAPH.LEFT
    p_info3.paragraph_format.left_indent = Cm(2.8)
    p_info3.paragraph_format.first_line_indent = Cm(0)
    p_info3.paragraph_format.space_before = Pt(2)
    p_info3.paragraph_format.space_after = Pt(2)
    p_info3.paragraph_format.line_spacing = 1.3
    r = p_info3.add_run("1. Trịnh Thành Đạt  —  MSSV: 1923050149 (Trưởng nhóm)\n2. Trần Trọng Đại  —  MSSV: 1923050745 (Thành viên)")
    r.font.name = "Times New Roman"
    r.font.size = Pt(13)

    p_info4 = doc.add_paragraph()
    p_info4.alignment = WD_ALIGN_PARAGRAPH.LEFT
    p_info4.paragraph_format.left_indent = Cm(2.0)
    p_info4.paragraph_format.first_line_indent = Cm(0)
    p_info4.paragraph_format.space_before = Pt(4)
    p_info4.paragraph_format.space_after = Pt(36)
    p_info4.paragraph_format.line_spacing = 1.3
    r = p_info4.add_run("Lớp học phần: 23DTH1 (Chuyên ngành: Công nghệ Thông tin)")
    r.font.name = "Times New Roman"
    r.font.size = Pt(13)

    p_foot = doc.add_paragraph()
    p_foot.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p_foot.paragraph_format.space_before = Pt(20)
    p_foot.paragraph_format.space_after = Pt(0)
    p_foot.paragraph_format.first_line_indent = Cm(0)
    r = p_foot.add_run("ĐỒNG NAI, NĂM 2026")
    r.font.name = "Times New Roman"
    r.font.size = Pt(13)
    r.bold = True

    # NGẮT SANG TRANG 2
    doc.add_page_break()

    # =========================================================================
    # TRANG 2: MỤC LỤC TỰ ĐỘNG (theo đúng yêu cầu)
    # =========================================================================
    p_toc_head = doc.add_paragraph()
    p_toc_head.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p_toc_head.paragraph_format.space_before = Pt(12)
    p_toc_head.paragraph_format.space_after = Pt(18)
    p_toc_head.paragraph_format.first_line_indent = Cm(0)
    r = p_toc_head.add_run("MỤC LỤC")
    r.font.name = "Times New Roman"
    r.font.size = Pt(14)
    r.bold = True
    add_bookmark(p_toc_head, "100", "_Toc_MUC_LUC")

    # Dynamic TOC Field Code
    p_dyn_toc = doc.add_paragraph()
    p_dyn_toc.paragraph_format.space_before = Pt(0)
    p_dyn_toc.paragraph_format.space_after = Pt(0)
    p_dyn_toc.paragraph_format.first_line_indent = Cm(0)
    add_dynamic_field(p_dyn_toc, r'TOC \o "1-3" \h \z \u')

    # Static Two-Layer TOC Rows đúng cấu trúc đề bài yêu cầu
    p_item = doc.add_paragraph()
    add_toc_line(p_item, "Mục lục hình ảnh", "3", "_Toc_MUC_LUC_HINH")
    
    p_item = doc.add_paragraph()
    add_toc_line(p_item, "Mục lục bảng biểu", "4", "_Toc_MUC_LUC_BANG")

    p_item = doc.add_paragraph()
    add_toc_line(p_item, "Đặt vấn đề bài toán", "5", "_Toc_DAT_VAN_DE")

    p_item = doc.add_paragraph()
    add_toc_line(p_item, "Các chức năng chính của đề tài", "6", "_Toc_CHUC_NANG_CHINH")

    p_item = doc.add_paragraph()
    add_toc_line(p_item, "Xây dựng Back End (MySQL và PHP)", "7", "_Toc_BACKEND")
    p_sub = doc.add_paragraph()
    p_sub.paragraph_format.left_indent = Cm(0.5)
    add_toc_line(p_sub, "Xây dựng mô hình DB", "7", "_Toc_MOHINH_DB")
    p_sub = doc.add_paragraph()
    p_sub.paragraph_format.left_indent = Cm(0.5)
    add_toc_line(p_sub, "API tương ứng các chức năng", "8", "_Toc_API_CHUC_NANG")

    p_item = doc.add_paragraph()
    add_toc_line(p_item, "Xây dựng Front-End (React Native)", "9", "_Toc_FRONTEND")
    p_sub = doc.add_paragraph()
    p_sub.paragraph_format.left_indent = Cm(0.5)
    add_toc_line(p_sub, "Màn hình giao diện thực thi", "9", "_Toc_GIAO_DIEN")

    # NGẮT SANG TRANG 3
    doc.add_page_break()

    # =========================================================================
    # TRANG 3: MỤC LỤC HÌNH ẢNH (theo đúng yêu cầu)
    # =========================================================================
    p_tof_head = doc.add_paragraph()
    p_tof_head.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p_tof_head.paragraph_format.space_before = Pt(12)
    p_tof_head.paragraph_format.space_after = Pt(18)
    p_tof_head.paragraph_format.first_line_indent = Cm(0)
    r = p_tof_head.add_run("MỤC LỤC HÌNH ẢNH")
    r.font.name = "Times New Roman"
    r.font.size = Pt(14)
    r.bold = True
    add_bookmark(p_tof_head, "101", "_Toc_MUC_LUC_HINH")

    # Dynamic TOF Field Code: TOC \h \z \c "Hình"
    p_dyn_tof = doc.add_paragraph()
    p_dyn_tof.paragraph_format.space_before = Pt(0)
    p_dyn_tof.paragraph_format.space_after = Pt(0)
    p_dyn_tof.paragraph_format.first_line_indent = Cm(0)
    add_dynamic_field(p_dyn_tof, r'TOC \h \z \c "Hình"')

    # Static Two-Layer TOF Rows
    p_item = doc.add_paragraph()
    add_toc_line(p_item, "Hình 1: Mô hình thực thể liên kết (ERD) Cơ sở dữ liệu MySQL", "7", "_Toc_Hinh_1")
    p_item = doc.add_paragraph()
    add_toc_line(p_item, "Hình 2: Giao diện quản trị CSDL trên phpMyAdmin máy chủ trường cấp", "7", "_Toc_Hinh_2")
    p_item = doc.add_paragraph()
    add_toc_line(p_item, "Hình 3: Kết quả thực thi API xác thực người dùng trên Postman", "8", "_Toc_Hinh_3")
    p_item = doc.add_paragraph()
    add_toc_line(p_item, "Hình 4: Kết quả thực thi API quản lý giao dịch thu chi trên Postman", "8", "_Toc_Hinh_4")
    p_item = doc.add_paragraph()
    add_toc_line(p_item, "Hình 5: Giao diện Màn hình Tổng quan và Số dư Đa Ví (React Native)", "9", "_Toc_Hinh_5")
    p_item = doc.add_paragraph()
    add_toc_line(p_item, "Hình 6: Giao diện Màn hình Thêm mới giao dịch thu chi nhanh", "9", "_Toc_Hinh_6")
    p_item = doc.add_paragraph()
    add_toc_line(p_item, "Hình 7: Giao diện Màn hình Báo cáo biểu đồ tròn và biểu đồ cột tháng", "10", "_Toc_Hinh_7")

    # NGẮT SANG TRANG 4
    doc.add_page_break()

    # =========================================================================
    # TRANG 4: MỤC LỤC BẢNG BIỂU (theo đúng yêu cầu)
    # =========================================================================
    p_tot_head = doc.add_paragraph()
    p_tot_head.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p_tot_head.paragraph_format.space_before = Pt(12)
    p_tot_head.paragraph_format.space_after = Pt(18)
    p_tot_head.paragraph_format.first_line_indent = Cm(0)
    r = p_tot_head.add_run("MỤC LỤC BẢNG BIỂU")
    r.font.name = "Times New Roman"
    r.font.size = Pt(14)
    r.bold = True
    add_bookmark(p_tot_head, "102", "_Toc_MUC_LUC_BANG")

    # Dynamic TOT Field Code: TOC \h \z \c "Bảng"
    p_dyn_tot = doc.add_paragraph()
    p_dyn_tot.paragraph_format.space_before = Pt(0)
    p_dyn_tot.paragraph_format.space_after = Pt(0)
    p_dyn_tot.paragraph_format.first_line_indent = Cm(0)
    add_dynamic_field(p_dyn_tot, r'TOC \h \z \c "Bảng"')

    # Static Two-Layer TOT Rows
    p_item = doc.add_paragraph()
    add_toc_line(p_item, "Bảng 1: Bảng tổng hợp các chức năng chính của ứng dụng di động", "6", "_Toc_Bang_1")
    p_item = doc.add_paragraph()
    add_toc_line(p_item, "Bảng 2: Đặc tả cấu trúc các bảng trong Cơ sở Dữ liệu MySQL", "7", "_Toc_Bang_2")
    p_item = doc.add_paragraph()
    add_toc_line(p_item, "Bảng 3: Danh mục các Endpoints RESTful API trên Backend PHP", "8", "_Toc_Bang_3")

    # NGẮT SANG TRANG 5
    doc.add_page_break()

    # =========================================================================
    # TRANG 5: ĐẶT VẤN ĐỀ BÀI TOÁN (Đúng 1 trang trọn vẹn theo yêu cầu)
    # =========================================================================
    p_dvd_head = doc.add_paragraph()
    p_dvd_head.alignment = WD_ALIGN_PARAGRAPH.LEFT
    p_dvd_head.paragraph_format.space_before = Pt(12)
    p_dvd_head.paragraph_format.space_after = Pt(12)
    p_dvd_head.paragraph_format.first_line_indent = Cm(0)
    r = p_dvd_head.add_run("ĐẶT VẤN ĐỀ BÀI TOÁN")
    r.font.name = "Times New Roman"
    r.font.size = Pt(14)
    r.bold = True
    add_bookmark(p_dvd_head, "103", "_Toc_DAT_VAN_DE")
    set_p_outline(p_dvd_head, 0)

    # Đoạn 1: Bối cảnh xã hội & thanh toán số
    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
    p.paragraph_format.space_before = Pt(6)
    p.paragraph_format.space_after = Pt(6)
    p.paragraph_format.line_spacing = 1.5
    p.paragraph_format.first_line_indent = Cm(1.5)
    r = p.add_run("Trong thời đại bùng nổ của công nghệ số và làn sóng thanh toán không dùng tiền mặt, các phương thức giao dịch hiện đại như chuyển khoản ngân hàng (VietQR, Napas 247), ví điện tử (MoMo, ZaloPay) và thẻ thanh toán đã trở thành thói quen phổ biến hàng ngày của người dân. Đặc biệt đối với các bạn trẻ, học sinh, sinh viên và những người mới đi làm, việc tiêu tiền qua tài khoản số mang lại sự tiện lợi, nhanh chóng nhưng cũng đồng thời tạo ra một hệ lụy tài chính đáng lo ngại: triệt tiêu \"ma sát thanh toán\" (Pain of Paying). Khi tiền không còn được trao tay bằng hiện vật vật lý, người dùng rất dễ đánh mất cảm giác kiểm soát số dư thực tế, dẫn đến các quyết định chi tiêu bốc đồng, mua sắm không kế hoạch và thường xuyên rơi vào cảnh thâm hụt tài chính, \"cháy túi\" trước khi kết thúc tháng.")
    r.font.name = "Times New Roman"
    r.font.size = Pt(13)

    # Đoạn 2: Bất cập của các giải pháp hiện tại
    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
    p.paragraph_format.space_before = Pt(6)
    p.paragraph_format.space_after = Pt(6)
    p.paragraph_format.line_spacing = 1.5
    p.paragraph_format.first_line_indent = Cm(1.5)
    r = p.add_run("Để giải quyết bài toán quản lý tài chính, nhiều người đã thử áp dụng việc ghi chép qua sổ tay hoặc tạo bảng tính Excel. Tuy nhiên, các phương pháp này đòi hỏi tính kiên trì cao, tốn kém thời gian và rất bất tiện khi cần cập nhật tức thì ngay sau mỗi giao dịch ngoài thực tế. Mặt khác, các ứng dụng quản lý chi tiêu hiện có trên thị trường di động (như Money Lover, Spendee hay Wallet) lại quá cồng kềnh, chứa đựng nhiều tính năng dư thừa không sát với nhu cầu thực tế của sinh viên (như quản lý danh mục đầu tư, chia tiền nhóm hay theo dõi cổ phiếu), đồng thời bị chèn quá nhiều quảng cáo gây gián đoạn trải nghiệm người dùng, hoặc đòi hỏi trả phí định kỳ để sử dụng các tính năng cơ bản.")
    r.font.name = "Times New Roman"
    r.font.size = Pt(13)

    # Đoạn 3: Mục tiêu đề tài di động
    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
    p.paragraph_format.space_before = Pt(6)
    p.paragraph_format.space_after = Pt(6)
    p.paragraph_format.line_spacing = 1.5
    p.paragraph_format.first_line_indent = Cm(1.5)
    r = p.add_run("Chính vì những lý do trên, đề tài \"Ứng dụng Quản lý Chi tiêu Cá nhân\" trên thiết bị di động được xây dựng với mục tiêu mang đến một giải pháp công nghệ tinh gọn, tốc độ và thân thiện. Ứng dụng tập trung giải quyết triệt để các nhu cầu cốt lõi nhất của người dùng: ghi chép nhanh chóng khoản thu/chi chỉ trong vài thao tác chạm, phân loại danh mục khoa học, thiết lập ngân sách chi tiêu tháng và trực quan hóa toàn bộ dòng tiền qua hệ thống biểu đồ tròn và biểu đồ cột rõ ràng. Được phát triển trên nền tảng React Native mượt mà kết hợp máy chủ Backend PHP và CSDL MySQL trên server trường cấp, đồ án không chỉ đáp ứng trọn vẹn yêu cầu học phần Lập trình Thiết bị Di động mà còn mang giá trị ứng dụng thiết thực cho đời sống hàng ngày.")
    r.font.name = "Times New Roman"
    r.font.size = Pt(13)

    # NGẮT SANG TRANG 6
    doc.add_page_break()

    # =========================================================================
    # TRANG 6: CÁC CHỨC NĂNG CHÍNH CỦA ĐỀ TÀI (Đúng 1 trang trọn vẹn theo yêu cầu)
    # =========================================================================
    p_cnc_head = doc.add_paragraph()
    p_cnc_head.alignment = WD_ALIGN_PARAGRAPH.LEFT
    p_cnc_head.paragraph_format.space_before = Pt(12)
    p_cnc_head.paragraph_format.space_after = Pt(12)
    p_cnc_head.paragraph_format.first_line_indent = Cm(0)
    r = p_cnc_head.add_run("CÁC CHỨC NĂNG CHÍNH CỦA ĐỀ TÀI")
    r.font.name = "Times New Roman"
    r.font.size = Pt(14)
    r.bold = True
    add_bookmark(p_cnc_head, "104", "_Toc_CHUC_NANG_CHINH")
    set_p_outline(p_cnc_head, 0)

    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
    p.paragraph_format.space_before = Pt(6)
    p.paragraph_format.space_after = Pt(6)
    p.paragraph_format.line_spacing = 1.5
    p.paragraph_format.first_line_indent = Cm(1.5)
    r = p.add_run("Đề tài ứng dụng quản lý chi tiêu cá nhân trên thiết bị di động được thiết kế và hiện thực hóa với 4 nhóm chức năng cốt lõi sau đây:")
    r.font.name = "Times New Roman"
    r.font.size = Pt(13)

    # 4 bullet items
    bullets = [
        ("Quản lý Tài khoản và Ví tài chính: ", "Cho phép người dùng đăng ký tài khoản, đăng nhập an toàn; theo dõi biến động số dư và quản lý độc lập 2 loại ví tiền tệ cơ bản gồm Ví tiền mặt (phục vụ tiêu dùng trực tiếp) và Ví ngân hàng (phục vụ chuyển khoản số)."),
        ("Ghi nhận khoản Thu và Chi tiêu: ", "Cung cấp biểu mẫu nhập liệu nhanh chóng với bàn phím số thuận tiện, cho phép nhập số tiền, chọn ngày giao dịch, phân loại vào các danh mục cụ thể (Ăn uống, Đi lại, Mua sắm, Tiền lương...) và lưu kèm ghi chú."),
        ("Thiết lập Ngân sách tháng: ", "Hỗ trợ người dùng chủ động cài đặt hạn mức chi tiêu dự kiến cho cả tháng hoặc theo từng danh mục, hiển thị thanh tiến độ cảnh báo trực quan đổi màu giúp người dùng nhận biết mức độ an toàn của dòng tiền."),
        ("Thống kê theo tháng và Biểu đồ chi tiêu: ", "Tổng hợp tự động tổng thu, tổng chi và số dư biến động theo từng tháng; hiển thị Biểu đồ tròn phản ánh tỷ trọng chi tiêu theo từng hạng mục và Biểu đồ cột so sánh tương quan giữa Thu và Chi.")
    ]

    for title_b, desc_b in bullets:
        p_b = doc.add_paragraph()
        p_b.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
        p_b.paragraph_format.left_indent = Cm(2.25)
        p_b.paragraph_format.first_line_indent = Cm(-0.35)
        p_b.paragraph_format.space_before = Pt(3)
        p_b.paragraph_format.space_after = Pt(3)
        p_b.paragraph_format.line_spacing = 1.5
        r_dash = p_b.add_run("- ")
        r_dash.font.name = "Times New Roman"
        r_dash.font.size = Pt(13)
        r_title = p_b.add_run(title_b)
        r_title.font.name = "Times New Roman"
        r_title.font.size = Pt(13)
        r_title.bold = True
        r_desc = p_b.add_run(desc_b)
        r_desc.font.name = "Times New Roman"
        r_desc.font.size = Pt(13)

    # Caption Bảng 1: Insert Caption SEQ Bảng
    p_cap_tbl = doc.add_paragraph()
    p_cap_tbl.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p_cap_tbl.paragraph_format.space_before = Pt(10)
    p_cap_tbl.paragraph_format.space_after = Pt(6)
    p_cap_tbl.paragraph_format.first_line_indent = Cm(0)
    add_bookmark(p_cap_tbl, "105", "_Toc_Bang_1")
    
    r_cap1 = p_cap_tbl.add_run("Bảng ")
    r_cap1.font.name = "Times New Roman"
    r_cap1.font.size = Pt(12)
    r_cap1.bold = True
    r_cap1.italic = True
    
    add_dynamic_field(p_cap_tbl, r'SEQ Bảng \* ARABIC', default_text="1", is_bold=True, is_italic=True)
    
    r_cap2 = p_cap_tbl.add_run(": Bảng tổng hợp các chức năng chính của ứng dụng di động")
    r_cap2.font.name = "Times New Roman"
    r_cap2.font.size = Pt(12)
    r_cap2.italic = True

    # Bảng 1: Academic Navy Theme
    tbl_data = [
        ["STT", "Chức năng chính", "Mô tả nghiệp vụ thực hiện", "Kết quả đầu ra"],
        ["01", "Quản lý Tài khoản & Ví", "Đăng ký, đăng nhập, theo dõi số dư Ví tiền mặt và Ngân hàng", "Hiển thị tổng tài sản và biến động số dư các ví"],
        ["02", "Ghi nhận Thu / Chi", "Nhập số tiền, chọn ngày giao dịch, danh mục, gán ghi chú", "Ghi nhận giao dịch, cập nhật số dư ví tức thì"],
        ["03", "Thiết lập Ngân sách", "Cài đặt hạn mức chi tiêu tối đa cho tháng", "Thanh tiến độ cảnh báo mức chi tiêu an toàn"],
        ["04", "Thống kê & Biểu đồ", "Lọc xem báo cáo tổng hợp theo từng tháng trong năm", "Biểu đồ tròn tỷ trọng chi tiêu & Biểu đồ cột Thu - Chi"]
    ]

    tbl = doc.add_table(rows=len(tbl_data), cols=4)
    tbl.alignment = WD_TABLE_ALIGNMENT.CENTER
    set_table_borders(tbl, color="CBD5E1", sz="4")

    col_widths = [Cm(1.2), Cm(4.2), Cm(5.3), Cm(4.8)]

    for r_idx, row in enumerate(tbl.rows):
        trPr = row._tr.get_or_add_trPr()
        trPr.append(parse_xml(f'<w:cantSplit {nsdecls("w")}/>'))
        
        is_header = (r_idx == 0)
        if is_header:
            trPr.append(parse_xml(f'<w:tblHeader {nsdecls("w")}/>'))

        for c_idx, cell in enumerate(row.cells):
            cell.width = col_widths[c_idx]
            cell.vertical_alignment = WD_ALIGN_VERTICAL.CENTER
            set_cell_margins(cell, top=100, bottom=100, left=140, right=140)
            
            p_cell = cell.paragraphs[0]
            p_cell.paragraph_format.space_before = Pt(2)
            p_cell.paragraph_format.space_after = Pt(2)
            p_cell.paragraph_format.line_spacing = 1.2
            p_cell.paragraph_format.first_line_indent = Cm(0)
            
            text_val = tbl_data[r_idx][c_idx]
            r_c = p_cell.add_run(text_val)
            r_c.font.name = "Times New Roman"
            
            if is_header:
                set_cell_shading(cell, "2B4C7E")
                r_c.font.size = Pt(12)
                r_c.font.color.rgb = RGBColor(255, 255, 255)
                r_c.bold = True
                p_cell.alignment = WD_ALIGN_PARAGRAPH.CENTER
            else:
                if r_idx % 2 == 0:
                    set_cell_shading(cell, "F8FAFC")
                else:
                    set_cell_shading(cell, "FFFFFF")
                
                r_c.font.size = Pt(11)
                r_c.bold = False
                r_c.font.color.rgb = RGBColor(0, 0, 0)
                
                if c_idx == 0:
                    p_cell.alignment = WD_ALIGN_PARAGRAPH.CENTER
                else:
                    p_cell.alignment = WD_ALIGN_PARAGRAPH.LEFT

    # NGẮT SANG TRANG 7 (Khung cho phần tiếp theo theo đúng file huongdanvietbaocao.txt)
    doc.add_page_break()

    # =========================================================================
    # TỪ TRANG 7: THEO ĐÚNG NGUYÊN VĂN YÊU CẦU CỦA THẦY TRONG HUONGDANVIETBAOCAO.TXT
    # =========================================================================
    # - Xây dựng Back End (MySQL và PHP)
    p_be = doc.add_paragraph()
    p_be.alignment = WD_ALIGN_PARAGRAPH.LEFT
    p_be.paragraph_format.space_before = Pt(12)
    p_be.paragraph_format.space_after = Pt(8)
    p_be.paragraph_format.first_line_indent = Cm(0)
    r = p_be.add_run("Xây dựng Back End (MySQL và PHP)")
    r.font.name = "Times New Roman"
    r.font.size = Pt(14)
    r.bold = True
    add_bookmark(p_be, "106", "_Toc_BACKEND")
    set_p_outline(p_be, 0)

    # → Xây dựng mô hình DB
    p_db = doc.add_paragraph()
    p_db.alignment = WD_ALIGN_PARAGRAPH.LEFT
    p_db.paragraph_format.space_before = Pt(6)
    p_db.paragraph_format.space_after = Pt(6)
    p_db.paragraph_format.first_line_indent = Cm(0)
    r = p_db.add_run("Xây dựng mô hình DB")
    r.font.name = "Times New Roman"
    r.font.size = Pt(13)
    r.bold = True
    add_bookmark(p_db, "107", "_Toc_MOHINH_DB")
    set_p_outline(p_db, 1)

    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
    p.paragraph_format.space_before = Pt(6)
    p.paragraph_format.space_after = Pt(6)
    p.paragraph_format.line_spacing = 1.5
    p.paragraph_format.first_line_indent = Cm(1.5)
    r = p.add_run("(Tại đây trình bày sơ đồ mô hình cơ sở dữ liệu MySQL trên máy chủ trường cấp https://103.139.203.43:887/phpmyadmin_cd13194c0d96d231/, cơ sở dữ liệu sql_nhom46_itimi gồm 5 bảng: users, wallets, categories, transactions, budgets...)")
    r.font.name = "Times New Roman"
    r.font.size = Pt(12)
    r.italic = True

    # → API tương ứng các chức năng
    p_api = doc.add_paragraph()
    p_api.alignment = WD_ALIGN_PARAGRAPH.LEFT
    p_api.paragraph_format.space_before = Pt(6)
    p_api.paragraph_format.space_after = Pt(6)
    p_api.paragraph_format.first_line_indent = Cm(0)
    r = p_api.add_run("API tương ứng các chức năng")
    r.font.name = "Times New Roman"
    r.font.size = Pt(13)
    r.bold = True
    add_bookmark(p_api, "108", "_Toc_API_CHUC_NANG")
    set_p_outline(p_api, 1)

    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
    p.paragraph_format.space_before = Pt(6)
    p.paragraph_format.space_after = Pt(6)
    p.paragraph_format.line_spacing = 1.5
    p.paragraph_format.first_line_indent = Cm(1.5)
    r = p.add_run("(Tại đây chụp ảnh thực thi API trên Postman hoặc chụp màn hình / copy code logic API PHP kết nối CSDL và xử lý các chức năng: Auth, Wallets, Categories, Transactions, Reports...)")
    r.font.name = "Times New Roman"
    r.font.size = Pt(12)
    r.italic = True

    # - Xây dựng Front-End (React native)
    p_fe = doc.add_paragraph()
    p_fe.alignment = WD_ALIGN_PARAGRAPH.LEFT
    p_fe.paragraph_format.space_before = Pt(14)
    p_fe.paragraph_format.space_after = Pt(8)
    p_fe.paragraph_format.first_line_indent = Cm(0)
    r = p_fe.add_run("Xây dựng Front-End (React native)")
    r.font.name = "Times New Roman"
    r.font.size = Pt(14)
    r.bold = True
    add_bookmark(p_fe, "109", "_Toc_FRONTEND")
    set_p_outline(p_fe, 0)

    # → Màn hình giao diện thực thi
    p_ui = doc.add_paragraph()
    p_ui.alignment = WD_ALIGN_PARAGRAPH.LEFT
    p_ui.paragraph_format.space_before = Pt(6)
    p_ui.paragraph_format.space_after = Pt(6)
    p_ui.paragraph_format.first_line_indent = Cm(0)
    r = p_ui.add_run("Màn hình giao diện thực thi")
    r.font.name = "Times New Roman"
    r.font.size = Pt(13)
    r.bold = True
    add_bookmark(p_ui, "110", "_Toc_GIAO_DIEN")
    set_p_outline(p_ui, 1)

    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
    p.paragraph_format.space_before = Pt(6)
    p.paragraph_format.space_after = Pt(6)
    p.paragraph_format.line_spacing = 1.5
    p.paragraph_format.first_line_indent = Cm(1.5)
    r = p.add_run("(Tại đây chụp ảnh giao diện thực thi của ứng dụng React Native trên thiết bị di động, đảm bảo phù hợp với các nội dung đã dạy trên lớp và phù hợp với các chức năng đã mô tả...)")
    r.font.name = "Times New Roman"
    r.font.size = Pt(12)
    r.italic = True

    # Output file path
    target_path = r"mobile-project/BaoCaoLTMB.docx"
    doc.save(target_path)
    print(f"Successfully generated {target_path}, size: {os.path.getsize(target_path)} bytes")

if __name__ == "__main__":
    main()
