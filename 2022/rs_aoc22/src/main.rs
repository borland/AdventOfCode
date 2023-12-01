mod day1_calorie_counting;

fn main() {
    match day1_calorie_counting::part2(&String::from("/Users/orion/Dev/aoc22/day1-input.txt")) {
        Ok(_) => {},
        Err(e) => {
            eprintln!("Error! {e}");
        }
    }
}
