import unittest
import os
import docx

class TestDocxAcademicFormat(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.docx_path = os.path.join(os.path.dirname(__file__), '..', 'files', 'KhoaLuanTotNghiep.docx')
        cls.doc = docx.Document(cls.docx_path)

    def test_headings_no_parentheses(self):
        """Bảo đảm tất cả tiêu đề Heading 1, 2, 3 không chứa dấu ngoặc đơn giải thích tiếng Anh."""
        headings_with_parens = []
        for i, p in enumerate(self.doc.paragraphs):
            if p.style.name.startswith('Heading'):
                text = p.text.strip()
                if '(' in text and ')' in text:
                    headings_with_parens.append((i, p.style.name, text))
        self.assertEqual(len(headings_with_parens), 0, f"Headings with parentheses found: {headings_with_parens}")

    def test_no_pain_point_buzzwords(self):
        """Bảo đảm văn bản sạch bóng cụm từ sáo rỗng 'điểm đau' hoặc 'pain point'."""
        buzzwords = ['điểm đau', 'pain point', 'painpoint', 'pain-point']
        found = []
        for i, p in enumerate(self.doc.paragraphs):
            t = p.text.lower()
            for b in buzzwords:
                if b in t:
                    found.append((f'P[{i}]', b, p.text[:60]))
        for t_idx, tbl in enumerate(self.doc.tables):
            for r_idx, row in enumerate(tbl.rows):
                for c_idx, cell in enumerate(row.cells):
                    t = cell.text.lower()
                    for b in buzzwords:
                        if b in t:
                            found.append((f'T[{t_idx}][{r_idx},{c_idx}]', b, cell.text[:60]))
        self.assertEqual(len(found), 0, f"Buzzwords found: {found}")

    def test_chapter_1_restructure(self):
        """Bảo đảm Chương 1 có đúng 4 đề mục cấp 2 tinh gọn."""
        c1_headings_l2 = []
        in_c1 = False
        for p in self.doc.paragraphs:
            if 'CHƯƠNG 1:' in p.text.upper() and p.style.name.startswith('Heading 1'):
                in_c1 = True
                continue
            if 'CHƯƠNG 2:' in p.text.upper() and p.style.name.startswith('Heading 1'):
                break
            if in_c1 and p.style.name.startswith('Heading 2'):
                c1_headings_l2.append(p.text.strip())

        expected_l2 = [
            '1.1 Khảo sát các quy trình quản lý tài chính hiện có',
            '1.2 Đánh giá các phương pháp và ứng dụng trên thị trường',
            '1.3 Nghiên cứu người dùng mục tiêu và các khó khăn thực tế',
            '1.4 Kết luận khảo sát hiện trạng và phát biểu bài toán'
        ]
        self.assertEqual(c1_headings_l2, expected_l2)

    def test_chapter_1_tables(self):
        """Bảo đảm 3 bảng trong Chương 1 nguyên vẹn, Bảng 1.2 có Money Lover."""
        t6 = self.doc.tables[6]  # Bảng 1.2
        app_names = [r.cells[0].text.strip() for r in t6.rows]
        self.assertIn('Money Lover', app_names)
        self.assertNotIn('Splitwise', app_names)

        t7 = self.doc.tables[7]  # Bảng 1.3
        headers = [c.text.strip() for c in t7.rows[0].cells]
        self.assertEqual(headers, ['Vấn đề', 'Biểu hiện', 'Tác động'])

    def test_bullet_indentation(self):
        """Kiểm tra quy định thụt lề bullet phân ý (- ) trong thân bài Chương 1."""
        bullet_found = False
        in_c1 = False
        for p in self.doc.paragraphs:
            if 'CHƯƠNG 1:' in p.text.upper() and p.style.name.startswith('Heading 1'):
                in_c1 = True
                continue
            if 'CHƯƠNG 2:' in p.text.upper() and p.style.name.startswith('Heading 1'):
                break
            if in_c1 and p.text.startswith('- '):
                bullet_found = True
                self.assertIsNotNone(p.paragraph_format.left_indent)
                self.assertIsNotNone(p.paragraph_format.first_line_indent)
                # Left indent ~ 0.75 cm, first line indent ~ -0.35 cm
                self.assertAlmostEqual(p.paragraph_format.left_indent.cm, 0.75, delta=0.05)
                self.assertAlmostEqual(p.paragraph_format.first_line_indent.cm, -0.35, delta=0.05)
        self.assertTrue(bullet_found)

if __name__ == '__main__':
    unittest.main()
