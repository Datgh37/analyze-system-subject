import os
from PIL import Image

Image.MAX_IMAGE_PIXELS = None
im = Image.open('files/ClassDiagram.drawio.png')
print(f"Loaded master image {im.size}")

# 1. Generate overview image for Section 4 Level 1
overview = im.copy()
# Resize to width 2400 for crisp display
overview.thumbnail((2400, 1800), Image.Resampling.LANCZOS)
overview.save('files/cd_overview.png')
print(f"Saved files/cd_overview.png, size={overview.size}")

# 2. Crop 6 contexts with high precision
containers = {
    'cd_context_1_identity': (80, 125, 1260, 860),
    'cd_context_2_wallet': (80, 1030, 1260, 2170),
    'cd_context_3_transactions': (1420, 1030, 1360, 2170),
    'cd_context_4_budgeting': (2860, 125, 1660, 860),
    'cd_context_5_debt_goals': (2860, 1030, 1660, 2170),
    'cd_context_6_nudge': (1420, 125, 1360, 860)
}

for name, (dx, dy, dw, dh) in containers.items():
    px1 = int(dx * 3 - 4) - 6
    py1 = int(dy * 3 - 105) - 6
    px2 = int((dx + dw) * 3 - 4) + 6
    py2 = int((dy + dh) * 3 - 105) + 6
    
    px1 = max(0, px1)
    py1 = max(0, py1)
    px2 = min(im.size[0], px2)
    py2 = min(im.size[1], py2)
    
    crop = im.crop((px1, py1, px2, py2))
    # If image is excessively large, scale so max dimension is around 2000-2400px (crystal sharp for 6 inches)
    if crop.size[0] > 2400 or crop.size[1] > 2400:
        ratio = min(2400 / crop.size[0], 2400 / crop.size[1])
        new_size = (int(crop.size[0] * ratio), int(crop.size[1] * ratio))
        crop = crop.resize(new_size, Image.Resampling.LANCZOS)
        
    out_path = f'files/{name}.png'
    crop.save(out_path)
    print(f"Saved {out_path}, size={crop.size}")
