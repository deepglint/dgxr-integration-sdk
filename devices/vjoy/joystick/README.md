vjoy
====

[vJoy][] library for [Go][]

[vJoy]: http://vjoystick.sf.net
[Go]: http://golang.org

Requirements
------------

[vJoy][] is required to use this library. Tested with vJoy 2.0.4 on Windows 7 x64.

Installation
------------

`go get github.com/tajtiattila/vjoy`

Usage
-----

The library provides direct access to the [vJoy][] interface library vJoyInderface.dll
via github.com/tajtiattila/vjoy/dll, along with a more idiomatic Device class
to update buttons and axes.

For further details see the Go documentation after installation.

Note: POV hats are not supported by [vJoy][] yet.
