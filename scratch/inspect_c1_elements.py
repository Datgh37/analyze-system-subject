import docx, sys

sys.stdout.reconfigure(encoding='utf-8')
doc = docx.Document('files/KhoaLuanTotNghiep.docx')

body = doc.element.body
c1_elem = doc.paragraphs[80]._p
c2_elem = doc.paragraphs[201]._p

recording = False
tbl_idx = 0
for child in body:
    if child == c1_elem:
        recording = True
        continue
    if child == c2_elem:
        break
    if recording and child.tag.endswith('tbl'):
        tbl_idx += 1
        t = docx.table.Table(child, doc)
        print(f"\n--- TABLE {tbl_idx} (rows: {len(t.rows)}, cols: {len(t.columns)}) ---")
        for r_idx, row in enumerate(t.rows):
            cells_text = [c.text.strip().replace('\n', ' ') for c in row.cells]
            print(f"Row {r_idx}: {cells_text}")