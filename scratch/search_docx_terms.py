import docx, sys
sys.stdout.reconfigure(encoding='utf-8')
doc = docx.Document('files/KhoaLuanTotNghiep.docx')

for i, p in enumerate(doc.paragraphs[80:201], start=80):
    t = p.text.strip()
    if 'Bảng 1.' in t:
        print(f'P[{i}]: {t}')
