import docx, sys
from docx.shared import Pt, Cm, Inches, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH

sys.stdout.reconfigure(encoding='utf-8')

doc_path = 'files/KhoaLuanTotNghiep.docx'
doc = docx.Document(doc_path)

print("Restoring original preamble layout (Roman numeral pages) in KhoaLuanTotNghiep.docx...")

for i, p in enumerate(doc.paragraphs):
    txt = p.text.strip()
    if not txt:
        continue
        
    # Only process front matter (before CHƯƠNG 1)
    if txt.startswith('CHƯƠNG 1'):
        print(f"Reached CHƯƠNG 1 at P{i}, stopping front matter restoration.")
        break
        
    # --- NHIỆM VỤ KHÓA LUẬN (P15 - P29) ---
    if 15 <= i < 30:
        pf = p.paragraph_format
        pf.first_line_indent = Inches(0)
        pf.line_spacing = 1.3
        if txt.startswith('NHIỆM VỤ'):
            pf.space_before = Pt(18)
            pf.space_after = Pt(12)
            p.alignment = WD_ALIGN_PARAGRAPH.CENTER
        elif txt.startswith('Chương '):
            pf.space_before = Pt(2)
            pf.space_after = Pt(2)
            p.alignment = WD_ALIGN_PARAGRAPH.LEFT
        elif any(txt.startswith(f'{n}.') for n in range(1, 6)):
            pf.space_before = Pt(6)
            pf.space_after = Pt(2)
            p.alignment = WD_ALIGN_PARAGRAPH.LEFT
        elif 'thông qua bộ môn' in txt:
            pf.space_before = Pt(4)
            pf.space_after = Pt(12)
            p.alignment = WD_ALIGN_PARAGRAPH.LEFT

    # --- LỜI CAM ĐOAN (P30 - P35) ---
    elif 30 <= i < 36:
        pf = p.paragraph_format
        if txt.startswith('LỜI CAM ĐOAN'):
            pf.space_before = Pt(18)
            pf.space_after = Pt(18)
            pf.line_spacing = 1.5
            pf.first_line_indent = Inches(0)
            p.alignment = WD_ALIGN_PARAGRAPH.CENTER
        elif 'sinh viên thực hiện' in txt.lower():
            # Date & Signature block
            pf.space_before = Pt(10)
            pf.space_after = Pt(0)
            pf.line_spacing = 1.3
            pf.first_line_indent = Inches(0)
            p.alignment = WD_ALIGN_PARAGRAPH.RIGHT
        else:
            # Body text
            pf.space_before = Pt(6)
            pf.space_after = Pt(12)
            pf.line_spacing = 1.5
            pf.first_line_indent = Cm(1.5)
            p.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY

    # --- LỜI CẢM ƠN (P36 - P41) ---
    elif 36 <= i < 42:
        pf = p.paragraph_format
        if txt.startswith('LỜI CẢM ƠN'):
            pf.space_before = Pt(18)
            pf.space_after = Pt(18)
            pf.line_spacing = 1.5
            pf.first_line_indent = Inches(0)
            p.alignment = WD_ALIGN_PARAGRAPH.CENTER
        elif 'sinh viên thực hiện' in txt.lower():
            # Date & Signature block
            pf.space_before = Pt(10)
            pf.space_after = Pt(0)
            pf.line_spacing = 1.3
            pf.first_line_indent = Inches(0)
            p.alignment = WD_ALIGN_PARAGRAPH.RIGHT
        else:
            # Body text
            pf.space_before = Pt(6)
            pf.space_after = Pt(12)
            pf.line_spacing = 1.5
            pf.first_line_indent = Cm(1.5)
            p.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY

    # --- NHẬN XÉT CỦA GIẢNG VIÊN HƯỚNG DẪN (P42 - P58) ---
    elif 42 <= i < 59:
        pf = p.paragraph_format
        pf.first_line_indent = Inches(0)
        pf.line_spacing = 1.3
        if txt.startswith('NHẬN XÉT'):
            pf.space_before = Pt(18)
            pf.space_after = Pt(18)
            pf.line_spacing = 1.5
            p.alignment = WD_ALIGN_PARAGRAPH.CENTER
        elif 'giảng viên hướng dẫn' in txt.lower() and 'đồng nai' in txt.lower():
            # Date & Instructor Signature block
            pf.space_before = Pt(10)
            pf.space_after = Pt(0)
            p.alignment = WD_ALIGN_PARAGRAPH.RIGHT
        elif any(txt.startswith(f'{n}.') for n in range(1, 5)):
            pf.space_before = Pt(6)
            pf.space_after = Pt(2)
            p.alignment = WD_ALIGN_PARAGRAPH.LEFT
        elif txt.startswith('- Những'):
            pf.space_before = Pt(2)
            pf.space_after = Pt(2)
            p.alignment = WD_ALIGN_PARAGRAPH.LEFT
        elif txt.startswith('...'):
            pf.space_before = Pt(2)
            pf.space_after = Pt(4)
            p.alignment = WD_ALIGN_PARAGRAPH.LEFT
        elif 'điểm đánh giá' in txt.lower():
            pf.space_before = Pt(4)
            pf.space_after = Pt(8)
            p.alignment = WD_ALIGN_PARAGRAPH.LEFT
        else:
            pf.space_before = Pt(3)
            pf.space_after = Pt(3)
            p.alignment = WD_ALIGN_PARAGRAPH.LEFT

    # --- MỤC LỤC & CÁC DANH MỤC (P59 - P71) ---
    elif 59 <= i < 72:
        pf = p.paragraph_format
        pf.first_line_indent = Inches(0)
        if txt.startswith('MỤC LỤC') or txt.startswith('DANH MỤC'):
            pf.space_before = Pt(18)
            pf.space_after = Pt(12)
            pf.line_spacing = 1.5
            p.alignment = WD_ALIGN_PARAGRAPH.CENTER
        elif txt.startswith('('):
            pf.space_before = Pt(6)
            pf.space_after = Pt(12)
            pf.line_spacing = 1.3
            p.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
            for r in p.runs:
                r.font.italic = True

# --- RESTORE TABLE 3 (Bảng từ viết tắt BM212) CỘT 0 IN ĐẬM ---
if len(doc.tables) > 3:
    tbl_abbr = doc.tables[3]
    print("Restoring bold for Column 0 (Abbreviations) in Table 3 (BM212)...")
    for row in tbl_abbr.rows[1:]:
        c0 = row.cells[0]
        for cp in c0.paragraphs:
            for cr in cp.runs:
                cr.font.bold = True

doc.save(doc_path)
print(f"Successfully saved and restored original front matter layout in {doc_path}!")
