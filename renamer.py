from PIL import Image
import os


def adjust_resolution(input_dir, output_dir, target_resolution):
    if not os.path.exists(output_dir):
        os.makedirs(output_dir)

    for filename in os.listdir(input_dir):
        if filename.endswith(".png"):
            input_path = os.path.join(input_dir, filename)
            output_path = os.path.join(output_dir, filename)

            img = Image.open(input_path)
            img = img.resize(target_resolution, Image.BILINEAR)
            img.save(output_path, "PNG")
            print(f"Adjusted resolution for {filename}")


if __name__ == "__main__":
    target_directory = "C:/development/aces-unity/Assets/Resources/Cards"
    output_directory = "C:/development/aces-unity/Assets/Resources/Cards"
    target_resolution = (200, 290)  # Set your desired resolution (width, height)

    adjust_resolution(target_directory, output_directory, target_resolution)
