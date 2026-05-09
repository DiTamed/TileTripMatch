# DESIGN

## Quyết định kiến trúc

Project được tách theo hướng đơn giản, dễ sửa trong Unity:

- Config: dùng LevelConfig (ScriptableObject) để lưu dữ liệu level.
- Models: giữ state thuần của game như BoardModel, SlotModel, TileModel.
- Presenters: chứa gameplay logic như chọn tile, đưa tile vào slot, match 3, thắng/thua.
- Views: chỉ lo hiển thị, animation, UI và input.
- GameplayBootstrapper: là entry point để ghép config -> model -> presenter -> view.
- GameEventBus: dùng event để giảm phụ thuộc trực tiếp giữa gameplay logic và UI/animation.

Mình chọn cách này vì scope bài test nhỏ nhưng vẫn cần tách rõ:

- Logic gameplay không bị dính chặt vào scene/UI.
- Level có thể author bằng asset thay vì hard-code.
- Dễ thay UI hoặc animation mà ít chạm vào rule game.

## Cách mình cấu trúc dữ liệu level

Mỗi level là một LevelConfig trong Assets/Resources/Levels.

LevelConfig hiện lưu:

- levelIndex: số level
- targetMatchCount: cần xóa bao nhiêu bộ 3 để thắng
- slotCount: số ô chứa ở thanh slot
- layerCount: số lớp tile
- symbolTypeCount: số loại icon dùng trong level
- timeLimitSeconds: thời gian giới hạn, 0 là không giới hạn
- tiles: danh sách TileData

Mỗi TileData gồm:

- symbol
- layer
- posX
- posY

Khi vào game, LevelGenerator chuyển tiles thành BoardModel, trong đó mỗi layer là một List<TileModel>. TileModel giữ cả vị trí hiển thị (PosX, PosY) lẫn vị trí logic (row, col) để presenter kiểm tra tile nào đang bị che.

## Làm thế nào để đảm bảo khả năng giải level

Hiện tại mình dùng cách "solvable by construction" ở mức đơn giản:

- Tổng số tile luôn là targetMatchCount * 3, nên toàn bộ level luôn chia hết thành các bộ 3.
- BuildSymbolPool tạo số lượng mỗi symbol theo bội số của 3, tránh sinh ra symbol lẻ không thể xóa hết.
- Tile được phân bổ theo layer và theo grid tương đối đều, nên luôn có các tile đang exposed để người chơi bắt đầu.
- Khi đưa vào slot, tile cùng symbol sẽ được chèn sát nhau (GetInsertIndex), giúp gom bộ 3 dễ hơn thay vì đẩy người chơi vào trạng thái ngẫu nhiên hoàn toàn.
- slotCount được tăng/giảm theo độ khó từng level để giữ cân bằng giữa thử thách và khả năng hoàn thành.

Điều quan trọng là: hệ hiện tại chưa có một solver đầy đủ để chứng minh 100% mọi level custom đều giải được. Nó đảm bảo khá tốt cho các level được generator tạo ra, nhưng với level thiết kế tay thì đây vẫn là heuristic, chưa phải formal validation.

## Trade-off và phần mình muốn cải thiện nếu có thêm thời gian

- Thêm một solver/offline validator: giả lập các nước đi hoặc generate level theo hướng reverse-play để có bảo chứng solvable rõ ràng hơn.
- Nâng overlap check: hiện logic che tile đang dựa trên row/col sau khi làm tròn, khá gọn nhưng còn thô nếu layout phức tạp hơn.
- Giảm phụ thuộc vào GameEventBus toàn cục: với project lớn hơn, mình sẽ bọc lifecycle/subscription chặt hơn để dễ test hơn.
- Thêm tooling cho level design: preview exposed tile, cảnh báo level bế tắc, và nút regenerate trong editor.
- Viết thêm test cho SlotPresenter, BoardPresenter, và generator để bắt lỗi regression sớm hơn.
