# Exp Standard Library — `system` Namespace

The `system` namespace is the core standard library of the Exp interpreter. It provides the essential runtime types, attributes, data structures, and helper functions used throughout Exp programs.

---

## 1. Attributes

### `ExpectFunc(name, paramsCount, stat)`
Tells the interpreter that a class must define a specific function.

### `Iteratable`
Marks a class as iterable via `foreach` loops. Required functions:
- `it_hasNext()`
- `it_next()`
- `it_reset()`


### `AllowMultiple`
Allows an attribute to be applied more than once.
---

## 2. Core Classes

---

### Array
A dynamic array type with functional helpers. An array size is final.

**Functions**
- `length()`
- `map(func)`
- `first(cond)`
- `findAll(cond)`
- `select(selector)`
- `ofType(type)`
- `copy()`
- `forEach(action)`

---

### string
A string class implemented over an internal array of characters.

**Functions**
- `charAt(i)`
- `length()`
- `substr(start, len)`
- `remove(start, count)` (`@ReadOnly`)
- `toCharArr()`
- `split(c)`
- `toNum()`

---

### Exception
The built‑in throwable type.

**Static Error Codes**
- `INDEX_OUT_OF_RANGE`
- `ARGUMENT_NULL`
- `NULL_REFERENCE`
- `INVALID_ARGUMENT`
- `INVALID_OPERATION`
- `EXTERN_OPERATION_FAILED`
- `NOT_FOUND`
- `DIVIDE_BY_ZERO`
- `STACK_OVERFLOW`

**Functions**
- `getType()`
- `getMessage()`
- `getData()`

---

### ExternTypeValue
Internal wrapper for external values. Includes a `@Translator` function.

---

### Type
Reflection helper.

**Static Functions**
- `Type.get(obj)`

---

### List
A dynamic list built on top of `Array`.

**Functions**
- `add(val?)`
- `remove(index)`
- `setAt(i, val?)`
- `get(i)`
- `first(cond)`
- `contains(val?)`
- `findAll(cond)`
- `clear()`
- `toArray()` (`@ReadOnly`)
- `count()`
- `indexOf(val?)`
- `toString()` (`@Translator`)
- `print()`

---

### Dictionary
A key–value store implemented using two `List`s.

**Functions**
- `set(key, val?)`
- `get(key)`
- `containsKey(key)`
- `toString()` (`@Translator`)

---

### Date
Represents a date/time.

**Constructors**
- `(year, month, day, hour, minute)`
- `(year, month, day, hour, minute, sec, millis, nanos)`
- `()`

**Functions**
- `setToNow()`
- `Date.now()` (static)
- `toString()` (`@Translator`)
- `toFullString()`

---

### cs
Interop helpers for C# types.

**Static Functions**
- `int(n)`
- `byte(n)`
- `float(n)`
- `long(n)`
- `action(f)`
- `exp(o)`

---

## 4. Global Functions

### Reference
- `refEquals(a, b)`

### Math
- `round(num)`
- `floor(num)`
- `pow(a, b)`
- `sign(n)`

### String Helpers
- `stringOf(obj?)`

### Console
- `println(text?)`

### Async
- `setTimeout(millis, action)`
- `runAsync(action, onComplete?)`
- `runAsync(action)`

---

## 5. Iterator Protocol

Classes marked with `@Iteratable` must define:
- `it_hasNext()`
- `it_next()`
- `it_reset()`

---

## 6. Summary

The `system` namespace provides:

- Core data structures: Array, List, Dictionary, string  
- Error handling: Exception, Throwable  
- Reflection: Type  
- Date/time: Date  
- Interop: cs  
- Async: setTimeout, runAsync  
- Math: round, floor, pow, sign  
- Interpreter attributes: ExpectFunc, Iteratable, Throwable  

It forms the foundation of the Exp interpreter runtime.
