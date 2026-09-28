# ĐẶC TẢ GIAO DIỆN & ATOMIC DESIGN SYSTEM — TRÌNH QUÉT HÓA ĐƠN OCR

## 1. Design Tokens Catalog
- **Colors:** Primary `#2563EB`, Scan Reticle `#10B981`, Warning `#F59E0B`, Danger `#EF4444`.
- **Typography:** Inter, Outfit. Mono font `JetBrains Mono` cho bảng giá và đối soát số học.
- **Spacing:** 8px grid (8px, 16px, 24px, 32px).

## 2. Atomic Component Hierarchy
- **Atoms:** `CameraViewfinderOverlay`, `ConfidenceScoreBadge`, `AuditStatusIcon`.
- **Molecules:** `ReceiptItemRow`, `KakeiboCategoryDropdown`.
- **Organisms:** `ReceiptScanReviewModal`, `ItemizedAuditTable`.
- **Templates:** `ReceiptScannerLayout`.

## 3. Accessibility & WCAG 2.1 AA Checklist
- **Contrast:** Tỷ lệ tương phản văn bản đạt 6.2:1 trên nền bảng xem xét.
- **Visual Feedback:** Âm thanh và rung phản hồi (Haptic) khi quét ảnh thành công.
