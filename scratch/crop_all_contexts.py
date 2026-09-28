import os
from PIL import Image

Image.MAX_IMAGE_PIXELS = None
im = Image.open('files/ClassDiagram.drawio.png')
print(f"Loaded image {im.size}")

# Containers from ClassDiagram.drawio:
# 1. Identity & Profile Context: x=80, y=125, w=1260, h=860
# 6. Decision Support & Nudge:   x=1420, y=125, w=1360, h=860
# 4. Budgeting & Adaptive DSA:   x=2860, y=125, w=1660, h=860
# 2. Account & Multi-Wallet:     x=80, y=1030, w=1260, h=2170
# 3. Transactions & AI OCR:      x=1420, y=1030, w=1360, h=2170
# 5. Personal Debt & Goals:      x=2860, y=1030, w=1660, h=2170

# Formula:
# scale = 3.0
# offset_x: drawio x=80 -> pixel x=236 -> offset_x = 236 - 80*3 = -4
# offset_y: drawio y=125 -> pixel y=270 -> offset_y = 270 - 125*3 = -105

containers = {
    'context_1_identity': (80, 125, 1260, 860),
    'context_2_multi_wallet': (80, 1030, 1260, 2170),
    'context_3_transactions_ocr': (1420, 1030, 1360, 2170),
    'context_4_budgeting_dsa': (2860, 125, 1660, 860),
    'context_5_debt_goals': (2860, 1030, 1660, 2170),
    'context_6_behavioral_nudge': (1420, 125, 1360, 860)
}

os.makedirs('scratch/cd_contexts', exist_ok=True)

for name, (dx, dy, dw, dh) in containers.items():
    # Convert to pixels with small padding
    px1 = int(dx * 3 - 4) - 10
    py1 = int(dy * 3 - 105) - 10
    px2 = int((dx + dw) * 3 - 4) + 10
    py2 = int((dy + dh) * 3 - 105) + 10
    
    # clamp
    px1 = max(0, px1)
    py1 = max(0, py1)
    px2 = min(im.size[0], px2)
    py2 = min(im.size[1], py2)
    
    crop = im.crop((px1, py1, px2, py2))
    
    # Save a high-res version for document
    out_path = f'scratch/cd_contexts/{name}.png'
    crop.save(out_path)
    
    # Also save a preview thumbnail to inspect
    thumb = crop.copy()
    thumb.thumbnail((600, 600))
    thumb.save(f'scratch/cd_contexts/{name}_thumb.png')
    
    print(f"Cropped {name}: box=({px1}, {py1}, {px2}, {py2}), size={crop.size}")
