# 🍊 # Mikan.Toolkit

**Mikan Toolkit** is a personal library I created to make working with **BASS**, **SDL**, and **Veldrid** easier for my own projects. It’s a toolkit, not a framework, and is designed to help with the specific tasks I need. While it’s not a comprehensive wrapper for all features of these libraries, it includes the parts that I found useful for my work. 

## What Mikan Is (and Isn’t)

Mikan is a toolkit I built for my own use, so it doesn’t aim to cover every feature of **BASS**, **SDL**, or **Veldrid**. I’ve only implemented the functionality that I actually needed for my projects. If you’re looking for a full binding of these libraries or every possible method, Mikan won’t provide that.

It's not a plug-and-play framework—some setup is needed, and the libraries work independently. This means you’ll have some flexibility in how you use it, but it might not always suit every use case.

## How It Works

Mikan offers a set of abstractions that make working with **BASS**, **SDL**, and **Veldrid** easier, especially if you prefer a more object-oriented approach. It manages handlers internally and exposes higher-level methods that are safer and easier to work with than the raw handlers you’d normally use.

However, not every feature of these libraries is implemented. Mikan is focused on the specific tasks I needed, so if you need more advanced functionality, you may need to interact directly with the underlying libraries.

## Libraries Used

- **BASS**: Used for audio processing, with only the necessary features exposed.
- **SDL**: Handles windowing and input, but again, only the relevant parts are included.
- **Veldrid**: Provides rendering capabilities, but only the features I required are wrapped.

## Cross-Platform & Mobile Support

Mikan is technically cross-platform, but I haven’t thoroughly tested it across all platforms. While mobile support is possible, it hasn’t been tested and I haven’t tried it myself.

## Why I Made This

I created Mikan because I wanted to simplify working with **BASS**, **SDL**, and **Veldrid** for my personal projects. It’s not designed to be a universal solution, just a toolkit that works for what I needed. I’ve included documentation in case others find it useful, but it’s very much a personal project that’s meant to save time and make development easier for me.
