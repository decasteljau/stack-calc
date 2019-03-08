# Stack Calc

The stack calculator can take math expressions at the input, and push the result of the expression to a global stack. It is then possible to trigger functions on the stack.

Expressions supported:

* Infix expressions with parenthesis and function calls.
* Function names, with no parenthesis. These will take their arguments from the stack.  

## Examples

| Function  | Result |
|---|---|
| 77 | will push 77 to the stack |
| 1+1 | will push 2 to the stack |
| 5*(3-abs(-5^2)) | will push -110 to the stack |
| + | will pop two numbers, perform their addition and push the result |
| sin | will pop 1 numbers, perform sinus and push the result |

## Functions supported

| Function  | Description  | Example | Result |
|---|---|---|---|
|+| Add | 2+2 | 4 |
|-| Subtract | 1-2 | -1 |
|*| Multiply | 4\*2 | 8 |
|/| Divide |  4/2 | 2 |
|^| Power | 3^2 | 9 |
|-| unary Minus| -5 | -5 |
|abs| Absolute value | abs(-3) | 3 |
|acos| Angle whose cosine is the specified number. | acos(-1) | 3.14159265358979 |
|asin| Angle whose sine is the specified number. |
|atan| Angle whose tangent is the specified number. |
|ceiling| Smallest integral value greater than or equal to the specified number. |
|combination| Combinations refer to the combination of n things taken k at a time without repetition |
|cos| Cosine of the specified angle. | cos(3.14159265358979) | -1 |
|cosh| Hyperbolic cosine of the specified angle. |
|exp| E raised to the specified power |
|factorial| Product of all positive integers less than or equal to n |
|floor| Largest integral value less than or equal to the specified number. | 3.4 | 3 |
|log| Logarithm of a specified number. |
|ln| Logarithm of a specified number with base e.|
|log10| Logarithm of a specified number with base 10. |
|min| Smaller of two specified numbers. |
|max| Larger of two specified numbers. |
|pow| Specified number raised to the specified power. |
|sin| Sine of the specified angle |
|sinh| Hyperbolic sine of the specified angle. |
|sqrt| Square root of a specified number. |
|tan| Tangent of the specified angle. |
|tanh| Hyperbolic tangent of the specified angle. |
|truncate| Integral part of a number. |

