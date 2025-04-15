#!/bin/bash

echo "🧹 开始清理损坏的 .meta 文件..."

# 所有待清理的 .meta 文件路径
meta_files=(
  "Assets/Resources/boss/bossanimation.meta"
  "Assets/Resources/boss/bossanimation/bossdash.meta"
  "Assets/Resources/boss/bossanimation/bossdash/boss_anim_dash.anim.meta"
  "Assets/Resources/boss/bossanimation/bossde.meta"
  "Assets/Resources/boss/bossanimation/bossde/boss_anim_default.anim.meta"
  "Assets/Resources/boss/bossanimation/bossdown.meta"
  "Assets/Resources/Systems/GlobalSystem.meta"
  "Assets/Scripts/GlobalSystem.meta"
  "Assets/Scripts/Level/BirthRoomTeleporter.meta"
  "Assets/Scripts/UI/DefeatMenu.meta"
  "Assets/Scripts/UI/MainMenu.meta"
  "Assets/Sprite/Boss 1.meta"
  "Assets/Sprite/Boss 1/DevilSnare.meta"
  "Assets/Sprite/boss/animation.meta"
  "Assets/Sprite/New Animator Controller.controller.meta"
  "Assets/Sprite/Treasure/treasure_0.controller.meta"
)

# 遍历删除
for file in "${meta_files[@]}"
do
  if [ -f "$file" ]; then
    rm "$file"
    echo "✅ 删除: $file"
  else
    echo "⚠️ 未找到: $file"
  fi
done

echo "🎉 清理完成！回到Unity，等待它重新生成 .meta 文件。"
