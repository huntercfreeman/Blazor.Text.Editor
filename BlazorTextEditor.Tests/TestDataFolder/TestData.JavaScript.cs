namespace BlazorTextEditor.Tests.TestDataFolder;

public static partial class TestData
{
    public static class JavaScript
    {
        public const string EXAMPLE_TEXT_28_LINES = @"// program to perform intersection between two arrays using Set
// intersection contains the elements of array1 that are also in array2

function performIntersection(arr1, arr2) {

    // converting into Set
    const setA = new Set(arr1);
    const setB = new Set(arr2);

    let intersectionResult = [];

    for (let i of setB) {
    
        if (setA.has(i)) {
            intersectionResult.push(i);
        }
        
    }
    
    return intersectionResult;

}

const array1 = [1, 2, 3, 5, 9];
const array2 = [1, 3, 5, 8];

const result = performIntersection(array1, array2);
console.log(result);";
    }
}