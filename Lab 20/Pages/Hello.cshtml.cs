// =====================================================================
// Pages/Hello.cshtml.cs — SESSION PROJECT (Rule 20/35/39/40)
// ITI Summer Training | Session 20 — ASP.NET Core Razor Pages
// Block 1 — the smallest possible proof that the URL is the file path.
//
// This file is a PAGE MODEL. A page model is an ordinary C# class that
// holds the data and the behaviour for exactly ONE page. It is the
// Razor Pages replacement for a controller — except a controller serves
// many pages and this class serves one.
//
// It is called a CODE-BEHIND file: the C# that sits behind one .cshtml
// file. Its file name is that .cshtml file's name with .cs added on the
// end, and that naming is not decoration — it is how the tooling pairs
// the two and nests them together in Solution Explorer.
//
// ⚠️ Pages/Hello.cshtml DOES NOT EXIST YET. You create it in TODO 3.
//    Until then this class compiles perfectly and is simply never used,
//    and Solution Explorer shows this file flat rather than nested under
//    a page. Both of those are expected, not broken.
// =====================================================================

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudentPortalWeb.Pages
{
    public class HelloModel : PageModel
    {
        // TODO 2 (part one): (Block 1.) Declare ONE public property on
        //         this class: a string, called Message, with both a
        //         getter and a setter, initialised to an empty string so
        //         it can never be null.

        public string Message { get; set; } = "";

        //
        //         ⚠️ It must be PUBLIC. A private field would compile
        //         fine here and then be invisible from the page in TODO
        //         3, with a red squiggle you would have no reason to
        //         expect. The page reaches this class from the outside,
        //         so anything the page needs to read must be public.
        //
        //         ⚠️ Notice what you are NOT writing. There is no
        //         view-model class, no ViewData entry, and nothing gets
        //         passed anywhere. In MVC you built an object and handed
        //         it to View(...). Here the page model IS the object,
        //         and the page will read this property straight off it.

        // TODO 2 (part two): (Block 1.) Underneath the property, write a
        //         method called OnGet — exactly that name, capital O,
        //         capital G — public, returning nothing, taking no
        //         parameters. Inside it, set Message to any sentence you
        //         like that says this page was reached without a
        //         controller.

        public void OnGet()
        {
            Message = "No Controller was involved in reaching this page";
        }

        //
        //         ⚠️ The NAME is the wiring. Nothing registers this
        //         method, no attribute marks it, no route mentions it.
        //         Razor Pages looks for a method called On + the HTTP
        //         verb, and an HTTP GET is what a browser sends when you
        //         type a URL and press Enter. Rename it OnGett and the
        //         page still loads, still returns 200 OK, and Message is
        //         still the empty string you initialised it to. Nothing
        //         throws. That is Block 1's mini-puzzle.
    }
}
