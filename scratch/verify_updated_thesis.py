import docx
from docx import Document

doc = Document('files/KhoaLuanTotNghiep.docx')
print(f"Tổng số đoạn văn (paragraphs): {len(doc.paragraphs)}")
print(f"Tổng số bảng (tables): {len(doc.tables)}")

print("\n--- 1. Kiểm tra Tên Đề Tài ---")
print(f"P9: {doc.paragraphs[9].text}")
print(f"P10: {doc.paragraphs[10].text}")
print(f"P11: {doc.paragraphs[11].text}")
print(f"BM205 (P24): {doc.paragraphs[24].text}")
print(f"BM208 (P50): {doc.paragraphs[50].text}")

print("\n--- 2. Kiểm tra Bảng Phân Công Nhiệm Vụ ---")
for i, t in enumerate(doc.tables):
    txt = ''.join(c.text for r in t.rows for c in r.cells)
    if 'trịnh thành đạt' in txt.lower():
        print(f"Tìm thấy Bảng phân công ở Table index {i}:")
        print(f"  Số dòng: {len(t.rows)}, Số cột: {len(t.columns)}")
        for r_idx, r in enumerate(t.rows):
            print(f"  Row {r_idx}: {[c.text.strip().replace(chr(10), ' ') for c in r.cells]}")
        break

print("\n--- 3. Kiểm tra Bullets ---")
checked = 0
for idx, p in enumerate(doc.paragraphs):
    txt = p.text.strip()
    if txt.startswith("- ") and idx > 78:
        pf = p.paragraph_format
        left = pf.left_indent.cm if pf.left_indent else None
        first = pf.first_line_indent.cm if pf.first_line_indent else None
        before = pf.space_before.pt if pf.space_before else None
        after = pf.space_after.pt if pf.space_after else None
        line = pf.line_spacing
        print(f"Bullet P{idx}: left={left}cm, first={first}cm, before={before}pt, after={after}pt, line={line} -> {txt[:50]}...")
        checked += 1
        if checked >= 6:
            break

print("\n--- 4. Kiểm tra Mục 2.5 Công nghệ dự kiến ---")
for idx, p in enumerate(doc.paragraphs):
    if "2.5" in p.text and "Hệ sinh thái Công nghệ" in p.text:
        print(f"P{idx} [{p.style.name}]: {p.text}")
        for j in range(idx+1, idx+9):
            print(f"  Sub P{j}: {doc.paragraphs[j].text[:80]}...")
        break

print("\n--- 5. Kiểm tra Section 3.3 Class Diagram (Ảnh & Văn xuôi) ---")
for idx, p in enumerate(doc.paragraphs):
    txt = p.text.strip()
    if "Sơ đồ kiến trúc tổng thể 6 Bounded Contexts" in txt or "Context 1:" in txt or "Context 6:" in txt:
        print(f"P{idx}: {txt}")

print("\n--- 6. Kiểm tra Mục 3.4, 3.5, 3.6 và Bookmarks ---")
for idx, p in enumerate(doc.paragraphs):
    txt = p.text.strip()
    if any(txt.startswith(f"3.{k}") for k in [4, 5, 6]):
        print(f"P{idx} [{p.style.name}]: {txt}")
        bm_names = [child.attrib.get('{http://schemas.openxmlformats.org/wordprocessingml/2006/main}name') for child in p._p if child.tag.endswith('bookmarkStart')]
        print(f"  Bookmarks: {bm_names}")
        for j in range(idx+1, min(idx+3, len(doc.paragraphs))):
            next_txt = doc.paragraphs[j].text.strip()
            print(f"  Next P{j} [{doc.paragraphs[j].style.name}]: {next_txt[:60] if next_txt else '(TRỐNG)'}")
