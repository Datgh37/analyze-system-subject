# -*- coding: utf-8 -*-
"""
Unit tests kiểm thử tự động hệ thống Caption Word (SEQ Bảng & SEQ Hình),
Bookmarks Cross-reference và trường Danh mục động TOF / TOT trong KhoaLuanTotNghiep.docx.
"""
import unittest
import os
import docx

class TestDocxCaptionsAndReferences(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.doc_path = os.path.join(os.path.dirname(__file__), "..", "files", "KhoaLuanTotNghiep.docx")
        cls.assertTrue(os.path.exists(cls.doc_path), f"File {cls.doc_path} must exist!")
        cls.doc = docx.Document(cls.doc_path)

    def test_caption_style_properties(self):
        """Kiểm tra style Caption tồn tại và tuân thủ định dạng Times New Roman, cỡ 12pt, nghiêng"""
        styles = [s.name for s in self.doc.styles]
        self.assertIn("Caption", styles)
        caption_style = self.doc.styles["Caption"]
        self.assertEqual(caption_style.font.name, "Times New Roman")
        self.assertTrue(caption_style.font.italic)

    def test_all_15_table_captions_exist_with_seq_fields(self):
        """Kiểm tra toàn bộ 15 Bảng biểu có Caption mang trường SEQ Bảng và format chuẩn"""
        seq_tables = []
        for p in self.doc.paragraphs:
            xml = p._p.xml
            if "SEQ Bảng" in xml:
                seq_tables.append((p.text.strip(), p.style.name))
        self.assertEqual(len(seq_tables), 15, f"Expected 15 table captions with SEQ Bảng, found {len(seq_tables)}")
        for text, style in seq_tables:
            self.assertEqual(style, "Caption")
            self.assertTrue(text.startswith("Bảng 1.") or text.startswith("Bảng 3."))

    def test_all_17_figure_captions_exist_with_seq_fields(self):
        """Kiểm tra toàn bộ 17 Hình ảnh có Caption mang trường SEQ Hình và format chuẩn"""
        seq_figures = []
        for p in self.doc.paragraphs:
            xml = p._p.xml
            if "SEQ Hình" in xml:
                seq_figures.append((p.text.strip(), p.style.name))
        self.assertEqual(len(seq_figures), 17, f"Expected 17 figure captions with SEQ Hình, found {len(seq_figures)}")
        for text, style in seq_figures:
            self.assertEqual(style, "Caption")
            self.assertTrue(text.startswith("Hình 3."))

    def test_cross_reference_bookmarks(self):
        """Kiểm tra tồn tại đủ 32 Bookmarks _Ref_Bang_* và _Ref_Hinh_* cho tính năng Cross-reference"""
        ref_bms = set()
        for p in self.doc.paragraphs:
            for bm in p._p.xpath('.//w:bookmarkStart'):
                name = bm.attrib.get('{http://schemas.openxmlformats.org/wordprocessingml/2006/main}name')
                if name and name.startswith("_Ref_"):
                    ref_bms.add(name)
        self.assertEqual(len(ref_bms), 32, f"Expected 32 reference bookmarks, found {len(ref_bms)}")
        
        # Verify specific key bookmarks
        self.assertIn("_Ref_Bang_1_1", ref_bms)
        self.assertIn("_Ref_Bang_1_3", ref_bms)
        self.assertIn("_Ref_Bang_3_1", ref_bms)
        self.assertIn("_Ref_Bang_3_12", ref_bms)
        self.assertIn("_Ref_Hinh_3_1", ref_bms)
        self.assertIn("_Ref_Hinh_3_17", ref_bms)

    def test_dynamic_tof_and_tot_fields_in_preamble(self):
        """Kiểm tra 2 trường mã TOC động cho Danh mục hình ảnh và Danh mục bảng biểu"""
        instr_texts = []
        for p in self.doc.paragraphs:
            for instr in p._p.xpath('.//w:instrText'):
                if instr.text:
                    instr_texts.append(instr.text.strip())
        
        self.assertTrue(any('TOC' in t and '"Hình"' in t for t in instr_texts), "TOC field for Hình must exist")
        self.assertTrue(any('TOC' in t and '"Bảng"' in t for t in instr_texts), "TOC field for Bảng must exist")

if __name__ == '__main__':
    unittest.main()
