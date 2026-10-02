import docx, sys

sys.stdout.reconfigure(encoding='utf-8')
doc = docx.Document('files/KhoaLuanTotNghiep.docx')

for idx in [81, 82]:
    p = doc.paragraphs[idx]
    pf = p.paragraph_format
    print(f"P[{idx}] ({p.style.name}): first_line_indent={pf.first_line_indent}, left_indent={pf.left_indent}, line_spacing={pf.line_spacing}, space_before={pf.space_before}, space_after={pf.space_after}")
