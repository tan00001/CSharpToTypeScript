# Generating Controls in a React Form

* For a `Enum` type properties of a C# class or a struct, a `select` control is generated.
* To generate a hidden input, you can use the `System.ComponentModel.DataAnnotations.UIHintAttribute`:
    ```
    public class CustomerAccount
    {
        public static readonly string[] AccountTypes = new string[]{ "checking", "saving" };

        [UIHint("hidden")]    
        public int Id { get; set; }

        public decimal? Balance { get; set; }
    }
    ```
* To generate a select control, you can use the `System.ComponentModel.DataAnnotations.UIHintAttribute`:
    ```
    public class CustomerAccount
    {
        [UIHint("select", "HTML", new object[] { "nameOfOptions", nameof(AccountTypes) })]
        public string Type { get; set; }

        public decimal? Balance { get; set; }
    }
    ```
    To specify the source of the options from another class, you can specify `"typeContainingOptions"`:
    ```
    public class Utilities
    {
        public static readonly string[] AccountTypes = new string[]{ "checking", "saving" };
    }

    public class CustomerAccount
    {
        [UIHint("select", "HTML", new object[] { "typeContainingOptions", typeof(Utilities), "nameOfOptions", nameof(AccountTypes) })]
        public string Type { get; set; }

        public decimal? Balance { get; set; }
    }
    ```
* Make an input control read-only, you can use the `System.ComponentModel.DataAnnotations.UIHintAttribute`:
```
    public class CustomerAccount
    {
        public static readonly string[] AccountTypes = new string[]{ "checking", "saving" };

        [UIHint("hidden")]    
        public int Id { get; set; }

        [UIHint("", "HTML",  new object[] { "readOny", true })]
        public decimal? Balance { get; set; }
    }
```
