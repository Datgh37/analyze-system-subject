import shutil, os, time, sys

sys.stdout.reconfigure(encoding='utf-8')

targets = [
    ('files/BaoCao_TienDo_GiuaKy_PTTKHTTT_v3.docx', 'files/BaoCao_TienDo_GiuaKy_PTTKHTTT.docx'),
    ('files/KhoaLuanTotNghiep_13pt_temp.docx', 'files/KhoaLuanTotNghiep.docx')
]

for src, dst in targets:
    if not os.path.exists(src):
        continue
    try:
        shutil.copyfile(src, dst)
        print(f"SUCCESS: Copied {src} -> {dst}")
        # Also update the official backup
        backup_name = dst.replace('.docx', '.backup.docx')
        shutil.copyfile(dst, backup_name)
        print(f"SUCCESS: Updated backup {backup_name}")
        os.remove(src)
    except PermissionError:
        print(f"LOCKED: Cannot overwrite {dst} because it is currently open in Word.")
