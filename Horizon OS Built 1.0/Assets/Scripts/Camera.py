import cv2
import mediapipe as mp

# Webcam
cap = cv2.VideoCapture(0)   # Menggunakan kamera dengan indeks 0
cap.set(3, 1280)            # Lebar frame
cap.set(4, 720)             # Tinggi frame

camera = None
width = 0
height = 0
mp_hands = None
hands = None

def set_camera(camera_unity, width_unity, height_unity):
    global camera, width, height, mp_hands, hands

    camera = camera_unity
    width = width_unity
    height = height_unity

    # Inisialisasi MediaPipe
    mp_hands = mp.solutions.hands
    hands = mp_hands.Hands(static_image_mode=False, max_num_hands=1, min_detection_confidence=0.9, min_tracking_confidence=0.9)

    # Variabel untuk menyimpan posisi landmark sebelumnya
    prev_middle_finger_tip = None
    prev_index_finger_tip = None
    prev_ring_finger_tip = None
    prev_pinky_finger_tip = None

def track_hand():
    global camera, mp_hands, hands

    # Ambil frame dari kamera
    success, img = camera.read()
    if not success:
        pass

    # Ubah gambar menjadi RGB (MediaPipe menggunakan gambar RGB)
    img_rgb = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)

    # Deteksi tangan
    results = hands.process(img_rgb)

    if results.multi_hand_landmarks:
        for hand_landmarks in results.multi_hand_landmarks:
            # Ambil nilai koordinat x, y, z dari landmark
            landmarks = []
            for lm in hand_landmarks.landmark:
                landmarks.append((lm.x, lm.y, lm.z))

            # Cek apakah gesture pinch terdeteksi
            thumb_tip = hand_landmarks.landmark[mp_hands.HandLandmark.THUMB_TIP]
            index_finger_tip = hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_TIP]
            middle_finger_tip = hand_landmarks.landmark[mp_hands.HandLandmark.MIDDLE_FINGER_TIP]
            ring_finger_tip = hand_landmarks.landmark[mp_hands.HandLandmark.RING_FINGER_TIP]
            pinky_tip = hand_landmarks.landmark[mp_hands.HandLandmark.PINKY_TIP]

            thumb_middle_distance = ((thumb_tip.x - middle_finger_tip.x) ** 2 + (thumb_tip.y - middle_finger_tip.y) ** 2) ** 0.5
            thumb_index_distance = ((thumb_tip.x - index_finger_tip.x) ** 2 + (thumb_tip.y - index_finger_tip.y) ** 2) ** 0.5
            thumb_ring_distance = ((thumb_tip.x - ring_finger_tip.x) ** 2 + (thumb_tip.y - ring_finger_tip.y) ** 2) ** 0.5
            thumb_pinky_distance = ((thumb_tip.x - pinky_tip.x) ** 2 + (thumb_tip.y - pinky_tip.y) ** 2) ** 0.5

            # Tentukan threshold untuk gesture pinch
            pinch_threshold = 0.1  # Sesuaikan threshold sesuai kebutuhan

            # MIDDLE FINGER
            if thumb_middle_distance < pinch_threshold:
                # Jika posisi landmark jari tengah sebelumnya telah tersedia
                if prev_middle_finger_tip:
                    # Hitung perpindahan landmark jari tengah
                    delta_x = round((middle_finger_tip.x - prev_middle_finger_tip.x) * 100)
                    delta_y = round((middle_finger_tip.y - prev_middle_finger_tip.y) * 100)
                    delta_z = round((middle_finger_tip.z - prev_middle_finger_tip.z) * 100)

                    # Cetak perpindahan landmark jari tengah
                    # print(f"Perpindahan Jari Tengah: dx={delta_x}, dy={delta_y}, dz={delta_z}")
                    return "PINCH MIDDLE"
                # Simpan posisi landmark jari tengah saat ini sebagai posisi sebelumnya
                prev_middle_finger_tip = middle_finger_tip

            # INDEX FINGER
            if thumb_index_distance < pinch_threshold:
                # Jika posisi landmark jari tengah sebelumnya telah tersedia
                if prev_index_finger_tip:
                    # Hitung perpindahan landmark jari tengah
                    delta_x = round((index_finger_tip.x - prev_index_finger_tip.x) * 100)
                    delta_y = round((index_finger_tip.y - prev_index_finger_tip.y) * 100)
                    delta_z = round((index_finger_tip.z - prev_index_finger_tip.z) * 100)

                    # Cetak perpindahan landmark jari tengah
                    # print(f"Perpindahan Jari Tengah: dx={delta_x}, dy={delta_y}, dz={delta_z}")
                    return "PINCH INDEX"
                # Simpan posisi landmark jari tengah saat ini sebagai posisi sebelumnya
                prev_index_finger_tip = index_finger_tip

            # RING FINGER
            if thumb_ring_distance < pinch_threshold:
                # Jika posisi landmark jari tengah sebelumnya telah tersedia
                if prev_ring_finger_tip:
                    # Hitung perpindahan landmark jari tengah
                    delta_x = round((ring_finger_tip.x - prev_ring_finger_tip.x) * 100)
                    delta_y = round((ring_finger_tip.y - prev_ring_finger_tip.y) * 100)
                    delta_z = round((ring_finger_tip.z - prev_ring_finger_tip.z) * 100)

                    # Cetak perpindahan landmark jari tengah
                    # print(f"Perpindahan Jari Tengah: dx={delta_x}, dy={delta_y}, dz={delta_z}")
                    return "PINCH RING"
                # Simpan posisi landmark jari tengah saat ini sebagai posisi sebelumnya
                prev_ring_finger_tip = ring_finger_tip

            # PINKY FINGER
            if thumb_pinky_distance < pinch_threshold:
                # Jika posisi landmark jari tengah sebelumnya telah tersedia
                if prev_pinky_finger_tip:
                    # Hitung perpindahan landmark jari tengah
                    delta_x = round((pinky_tip.x - prev_pinky_finger_tip.x) * 100)
                    delta_y = round((pinky_tip.y - prev_pinky_finger_tip.y) * 100)
                    delta_z = round((pinky_tip.z - prev_pinky_finger_tip.z) * 100)

                    # Cetak perpindahan landmark jari tengah
                    # print(f"Perpindahan Jari Tengah: dx={delta_x}, dy={delta_y}, dz={delta_z}")
                    return "PINCH PINKY"
                # Simpan posisi landmark jari tengah saat ini sebagai posisi sebelumnya
                prev_pinky_finger_tip = pinky_tip

    # Tampilkan gambar
    # cv2.imshow("Gambar", img)

    # Tekan 'q' untuk keluar dari loop
    # if cv2.waitKey(1) & 0xFF == ord('q'):
    #     return

# Bebaskan sumber daya dan tutup jendela OpenCV
cap.release()
cv2.destroyAllWindows()

