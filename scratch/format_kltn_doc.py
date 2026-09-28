import docx
from docx.shared import Pt, Cm, Inches, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH

src_path = 'files/KhoaLuanTotNghiep.docx'
doc = docx.Document(src_path)

body_updated = 0
heading_updated = 0
caption_updated = 0

for i, p in enumerate(doc.paragraphs):
    txt = p.text.strip()
    if not txt:
        continue
    
    # 1. Skip Cover Pages (P0 - P29)
    if i < 30:
        continue
        
    # 2. Preamble Body (Lời cam đoan, Lời cảm ơn, etc. P30 - P71)
    if 30 <= i < 72:
        if txt.startswith('LỜI ') or txt.startswith('NHẬN XÉT') or txt.startswith('MỤC LỤC') or txt.startswith('DANH MỤC'):
            p.paragraph_format.space_before = Pt(18)
            p.paragraph_format.space_after = Pt(12)
            p.paragraph_format.line_spacing = 1.5
            p.paragraph_format.first_line_indent = Inches(0)
            p.paragraph_format.keep_with_next = True
            for r in p.runs:
                r.font.name = 'Times New Roman'
                r.font.size = Pt(14)
                r.font.bold = True
                r.font.color.rgb = RGBColor(0, 0, 0)
            heading_updated += 1
        elif 'sinh viên' in txt.lower() or 'đồng nai, ngày' in txt.lower() or 'giảng viên hướng dẫn' in txt.lower():
            p.paragraph_format.line_spacing = 1.5
            continue
        elif len(txt) > 30:
            # Narrative paragraphs in Cam doan / Cam on
            p.paragraph_format.line_spacing = 1.5
            p.paragraph_format.space_before = Pt(6)
            p.paragraph_format.space_after = Pt(6)
            p.paragraph_format.first_line_indent = Cm(1.5)
            p.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
            for r in p.runs:
                r.font.name = 'Times New Roman'
                r.font.size = Pt(13)
                r.font.color.rgb = RGBColor(0, 0, 0)
            body_updated += 1
        continue

    # 3. Chapters & Main Content (i >= 72)
    if txt.startswith('CHƯƠNG'):
        p.paragraph_format.space_before = Pt(18)
        p.paragraph_format.space_after = Pt(12)
        p.paragraph_format.line_spacing = 1.5
        p.paragraph_format.first_line_indent = Inches(0)
        p.paragraph_format.keep_with_next = True
        for r in p.runs:
            r.font.name = 'Times New Roman'
            r.font.size = Pt(14)
            r.font.bold = True
            r.font.color.rgb = RGBColor(0, 0, 0)
        heading_updated += 1
    elif any(txt.startswith(f'{c}.') and len(txt) > 2 and txt[2].isdigit() for c in range(1, 6)):
        # Heading 2 (X.Y) or Heading 3 (X.Y.Z)
        dots = txt.split()[0].count('.')
        p.paragraph_format.line_spacing = 1.5
        p.paragraph_format.first_line_indent = Inches(0)
        p.paragraph_format.keep_with_next = True
        if dots == 1:
            p.paragraph_format.space_before = Pt(12)
            p.paragraph_format.space_after = Pt(6)
        else:
            p.paragraph_format.space_before = Pt(6)
            p.paragraph_format.space_after = Pt(6)
        for r in p.runs:
            r.font.name = 'Times New Roman'
            r.font.size = Pt(13)
            r.font.bold = True
            r.font.color.rgb = RGBColor(0, 0, 0)
        heading_updated += 1
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
            r.font.italic = True
            r.font.color.rgb = RGBColor(0, 0, 0)
        caption_updated += 1
    elif txt.startswith('[ KHUNG'):
        continue
    elif txt.startswith('TIẾN ĐỘ') or txt.startswith('TÀI LIỆU THAM KHẢO'):
        p.paragraph_format.space_before = Pt(18)
        p.paragraph_format.space_after = Pt(12)
        p.paragraph_format.line_spacing = 1.5
        p.paragraph_format.first_line_indent = Inches(0)
        p.paragraph_format.keep_with_next = True
        for r in p.runs:
            r.font.name = 'Times New Roman'
            r.font.size = Pt(14)
            r.font.bold = True
            r.font.color.rgb = RGBColor(0, 0, 0)
        heading_updated += 1
    elif txt.startswith('[') and ']' in txt and len(txt) > 20:
        # Reference item
        p.paragraph_format.line_spacing = 1.5
        p.paragraph_format.space_before = Pt(3)
        p.paragraph_format.space_after = Pt(3)
        p.paragraph_format.first_line_indent = Inches(0)
        p.alignment = WD_ALIGN_PARAGRAPH.LEFT
        for r in p.runs:
            r.font.name = 'Times New Roman'
            r.font.size = Pt(12)
            r.font.color.rgb = RGBColor(0, 0, 0)
    else:
        # Narrative body text
        p.paragraph_format.line_spacing = 1.5
        p.paragraph_format.space_before = Pt(6)
        p.paragraph_format.space_after = Pt(6)
        p.paragraph_format.first_line_indent = Cm(1.5)
        p.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
        for r in p.runs:
            r.font.name = 'Times New Roman'
            r.font.size = Pt(13)
            r.font.color.rgb = RGBColor(0, 0, 0)
        body_updated += 1

doc.save(src_path)
print(f'Successfully formatted {src_path}:')
print(f'- {body_updated} body paragraphs formatted (1.5 lines, before/after 6pt, tab 1.5cm, 13pt)')
print(f'- {heading_updated} headings formatted (size 14/13, bold, 1.5 lines)')
print(f'- {caption_updated} captions formatted (size 11.5, italic, center)')
